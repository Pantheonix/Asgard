import 'dart:io';

import 'package:cqrs_mediator/cqrs_mediator.dart';
import 'package:dartz/dartz.dart';
import 'package:hermes_tests/api/core/hermes.pb.dart';
import 'package:hermes_tests/domain/entities/test_metadata.dart';
import 'package:hermes_tests/domain/exceptions/storage_failures.dart';

class DefragmentTestAsyncQuery
    extends IAsyncQuery<Either<StorageFailure, TestMetadata>> {
  final Metadata testMetadata;
  final Stream<Chunk> inputStream;
  final String destTestRootFolderForChunkedTest;
  final String destTestRootFolderForArchivedTest;
  final int maxTestSize;

  DefragmentTestAsyncQuery({
    required this.testMetadata,
    required this.inputStream,
    required this.destTestRootFolderForChunkedTest,
    required this.destTestRootFolderForArchivedTest,
    required this.maxTestSize,
  });
}

class DefragmentTestAsyncQueryHandler extends IAsyncQueryHandler<
    Either<StorageFailure, TestMetadata>, DefragmentTestAsyncQuery> {
  @override
  Future<Either<StorageFailure, TestMetadata>> call(
    DefragmentTestAsyncQuery command,
  ) async {
    if (command.testMetadata.testSize > command.maxTestSize) {
      return left(
        StorageFailure.testSizeLimitExceeded(
          message:
              'Test size ${command.testMetadata.testSize}B exceeds max size ${command.maxTestSize}B',
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
      await command.inputStream.forEach((chunk) {
        writtenBytes += chunk.data.length;
        if (writtenBytes > command.testMetadata.testSize) {
          throw Exception('Received more bytes than expected metadata size: '
              '${writtenBytes}B > ${command.testMetadata.testSize}B');
        }

        sink.add(chunk.data);
      });
    } catch (e) {
      await sink.close();
      _disposeLocalAsset(resultTestMetadata.archivedTestPath);

      return left(
        StorageFailure.testSizeLimitExceeded(
          message: 'Invalid test file ${resultTestMetadata.archivedTestPath}}',
        ),
      );
    }

    await sink.close();

    if (!_isZipFile(resultTestMetadata.archivedTestPath)) {
      _disposeLocalAsset(resultTestMetadata.archivedTestPath);
      return left(
        StorageFailure.invalidLocalTestFormat(
          message:
              'Non-zip or tampered test file ${resultTestMetadata.archivedTestPath}}',
        ),
      );
    }

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
