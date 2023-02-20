import 'dart:io';

import 'package:cqrs_mediator/cqrs_mediator.dart';
import 'package:dartz/dartz.dart';
import 'package:hermes_tests/api/core/hermes.pb.dart';
import 'package:hermes_tests/domain/entities/test_metadata.dart';
import 'package:hermes_tests/domain/exceptions/storage_failures.dart';
import 'package:logger/logger.dart';

class DefragmentTestAsyncQuery
    extends IAsyncQuery<Either<StorageFailure, TestMetadata>> {
  final Metadata testMetadata;
  final Stream<Chunk> chunkStream;
  final String destTestRootFolderForChunkedTest;
  final String destTestRootFolderForArchivedTest;
  final int maxTestSize;

  DefragmentTestAsyncQuery({
    required this.testMetadata,
    required this.chunkStream,
    required this.destTestRootFolderForChunkedTest,
    required this.destTestRootFolderForArchivedTest,
    required this.maxTestSize,
  });
}

class DefragmentTestAsyncQueryHandler extends IAsyncQueryHandler<
    Either<StorageFailure, TestMetadata>, DefragmentTestAsyncQuery> {
  final Logger _logger;

  DefragmentTestAsyncQueryHandler(
    this._logger,
  );

  @override
  Future<Either<StorageFailure, TestMetadata>> call(
    DefragmentTestAsyncQuery command,
  ) async {
    _logger.i(
      'Calling Defragment UseCase for test ${command.testMetadata.testId}...',
    );

    if (command.testMetadata.testSize > command.maxTestSize) {
      final message =
          'Test size ${command.testMetadata.testSize}B exceeds max size ${command.maxTestSize}B';
      _logger.e(message);

      return left(
        StorageFailure.testSizeLimitExceeded(
          message: message,
        ),
      );
    }

    final TestMetadata resultTestMetadata = TestMetadata(
      problemId: command.testMetadata.problemId,
      testId: command.testMetadata.testId,
      srcTestRootFolder: command.destTestRootFolderForChunkedTest,
      destTestRootFolder: command.destTestRootFolderForArchivedTest,
    );

    final File outputDefragmentedArchivedTestFile =
        await File(resultTestMetadata.archivedTestPath).create(recursive: true);
    final IOSink sink = outputDefragmentedArchivedTestFile.openWrite();

    int writtenBytes = 0;
    try {
      await command.chunkStream.forEach((chunk) {
        writtenBytes += chunk.data.length;
        _logger.i(
          '$writtenBytes bytes written for test ${resultTestMetadata.testRelativePath}',
        );
        if (writtenBytes > command.testMetadata.testSize) {
          throw Exception(
            'Received more bytes than expected metadata size: '
            '${writtenBytes}B > ${command.testMetadata.testSize}B for test ${resultTestMetadata.testRelativePath}',
          );
        }

        sink.add(chunk.data);
      });
    } catch (e) {
      await sink.close();
      _disposeLocalAsset(resultTestMetadata.archivedTestPath);
      _logger.e(e.toString());

      return left(
        StorageFailure.testSizeLimitExceeded(
          message: e.toString(),
        ),
      );
    }

    await sink.close();
    _logger.i('Output file stream closed');

    if (!_isZipFile(resultTestMetadata.archivedTestPath)) {
      _disposeLocalAsset(resultTestMetadata.archivedTestPath);
      final message =
          'Non-zip or tampered test file ${resultTestMetadata.archivedTestPath}';
      _logger.e(message);

      return left(
        StorageFailure.invalidLocalTestFormat(
          message: message,
        ),
      );
    }
    _logger.i(
      'Test defragmented and saved to ${resultTestMetadata.archivedTestPath}',
    );

    return right(resultTestMetadata);
  }
}

bool _isZipFile(String filePath) {
  final File file = File(filePath);
  final List<int> bytes = file.readAsBytesSync();
  final String fileExtension = file.path.split('.').last;

  return fileExtension == 'zip' &&
      bytes[0] == 0x50 &&
      bytes[1] == 0x4B &&
      bytes[2] == 0x03 &&
      bytes[3] == 0x04;
}

void _disposeLocalAsset(String path) {
  final File file = File(path);
  if (file.existsSync()) {
    file.deleteSync();
  }
}
