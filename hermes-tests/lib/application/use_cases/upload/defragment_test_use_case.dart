import 'dart:io';

import 'package:cqrs_mediator/cqrs_mediator.dart';
import 'package:dartz/dartz.dart';
import 'package:hermes_tests/domain/entities/test_metadata.dart';
import 'package:hermes_tests/domain/exceptions/storage_failures.dart';
import 'package:logger/logger.dart';
import 'package:path/path.dart' as path;

class DefragmentTestAsyncQuery
    extends IAsyncQuery<Either<StorageFailure, Unit>> {
  final TestMetadata testMetadata;

  DefragmentTestAsyncQuery({
    required this.testMetadata,
  });
}

class DefragmentTestAsyncQueryHandler extends IAsyncQueryHandler<
    Either<StorageFailure, Unit>, DefragmentTestAsyncQuery> {
  final Logger _logger;

  DefragmentTestAsyncQueryHandler(
    this._logger,
  );

  @override
  Future<Either<StorageFailure, Unit>> call(
    DefragmentTestAsyncQuery command,
  ) async {
    return command.testMetadata.maybeMap(
      testToDefragment: (testMetadata) async {
        final testRelativePath = path.join(
          testMetadata.problemId,
          testMetadata.testId,
        );
        _logger.i(
          'Calling Defragment UseCase for test $testRelativePath...',
        );

        if (testMetadata.testSize > testMetadata.maxTestSize) {
          final message =
              'Test size ${testMetadata.testSize}B exceeds max size ${testMetadata.maxTestSize}B';
          _logger.e(message);

          return left(
            StorageFailure.testSizeLimitExceeded(
              message: message,
            ),
          );
        }

        final localArchivedTestFilePath = path.join(
          testMetadata.toDir,
          '$testRelativePath.${testMetadata.archiveTypeExtension}',
        );
        final File localArchivedTestFile = await File(
          localArchivedTestFilePath,
        ).create(
          recursive: true,
        );
        final IOSink outputFileSink = localArchivedTestFile.openWrite();

        int writtenBytes = 0;
        try {
          await testMetadata.chunkStream.forEach((chunk) {
            writtenBytes += chunk.data.length;
            _logger.i(
              '$writtenBytes bytes written for test $testRelativePath',
            );
            if (writtenBytes > testMetadata.testSize) {
              throw Exception(
                'Received more bytes than expected metadata size: '
                '${writtenBytes}B > ${testMetadata.testSize}B for test ${testRelativePath}',
              );
            }

            outputFileSink.add(chunk.data);
          });
        } catch (e) {
          await outputFileSink.close();
          _disposeLocalAsset(localArchivedTestFilePath);
          _logger.e(e.toString());

          return left(
            StorageFailure.testSizeLimitExceeded(
              message: e.toString(),
            ),
          );
        }

        await outputFileSink.close();
        _logger.i('Output file stream closed');

        if (!_isZipFile(localArchivedTestFilePath)) {
          _disposeLocalAsset(localArchivedTestFilePath);
          final message =
              'Non-zip or tampered test file $localArchivedTestFilePath';
          _logger.e(message);

          return left(
            StorageFailure.invalidLocalTestFormat(
              message: message,
            ),
          );
        }
        _logger.i(
          'Test defragmented and saved to $localArchivedTestFilePath',
        );

        return right(unit);
      },
      orElse: () => left(
        StorageFailure.unexpected(
          message: 'Invalid test metadata passed to DefragmentTestUseCase',
        ),
      ),
    );
  }
}

bool _isZipFile(String filePath) {
  final File file = File(filePath);
  final List<int> bytes = file.readAsBytesSync();
  if (bytes.isEmpty) {
    return false;
  }

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
