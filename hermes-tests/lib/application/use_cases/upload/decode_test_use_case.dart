import 'dart:io';

import 'package:cqrs_mediator/cqrs_mediator.dart';
import 'package:dartz/dartz.dart';
import 'package:hermes_tests/domain/entities/test_metadata.dart';
import 'package:hermes_tests/domain/exceptions/storage_failures.dart';
import 'package:archive/archive_io.dart';
import 'package:logger/logger.dart';
import 'package:path/path.dart' as path;

class DecodeTestAsyncQuery extends IAsyncQuery<Either<StorageFailure, Unit>> {
  final TestMetadata testMetadata;

  DecodeTestAsyncQuery({
    required this.testMetadata,
  });
}

class DecodeTestAsyncQueryHandler extends IAsyncQueryHandler<
    Either<StorageFailure, Unit>, DecodeTestAsyncQuery> {
  final Logger _logger;

  DecodeTestAsyncQueryHandler(
    this._logger,
  );

  @override
  Future<Either<StorageFailure, Unit>> call(
    DecodeTestAsyncQuery command,
  ) async {
    return command.testMetadata.maybeMap(
      testToDecode: (testMetadata) async {
        final testRelativePath = path.join(
          testMetadata.problemId,
          testMetadata.testId,
        );
        _logger.i(
          'Calling Decode UseCase for test $testMetadata...',
        );

        // check if archived test file exists
        final localArchivedTestFilePath = path.join(
          testMetadata.fromDir,
          '$testRelativePath.${testMetadata.archiveTypeExtension}',
        );
        final File localArchivedTestFile = File(
          localArchivedTestFilePath,
        );

        if (localArchivedTestFile.existsSync() == false) {
          final message = 'Archived test file not found for test $testMetadata';
          _logger.e(message);

          return left(
            StorageFailure.localTestNotFound(
              message: message,
            ),
          );
        }

        try {
          final archivedTestInputStream = InputFileStream(
            localArchivedTestFilePath,
          );
          final archive = ZipDecoder().decodeBuffer(archivedTestInputStream);

          final localUnarchivedTestFolderPath = path.join(
            testMetadata.toDir,
            testRelativePath,
          );
          extractArchiveToDisk(
            archive,
            localUnarchivedTestFolderPath,
          );
        } catch (e) {
          _logger.e(
            '${e.toString()} when uploading test $testMetadata',
          );

          return Future.value(
            left(
              StorageFailure.invalidLocalTestFormat(
                message: 'Invalid local test format for test $testMetadata',
              ),
            ),
          );
        }

        _logger.i(
          'Test $testRelativePath decoded successfully',
        );

        return right(unit);
      },
      orElse: () => left(
        StorageFailure.unexpected(
          message: 'Invalid test metadata passed to DecodeTestUseCase',
        ),
      ),
    );
  }
}
