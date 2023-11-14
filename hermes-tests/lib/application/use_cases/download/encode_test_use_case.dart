import 'dart:io';

import 'package:archive/archive_io.dart';
import 'package:cqrs_mediator/cqrs_mediator.dart';
import 'package:dartz/dartz.dart';
import 'package:hermes_tests/domain/entities/test_metadata.dart';
import 'package:hermes_tests/domain/exceptions/storage_failures.dart';
import 'package:logger/logger.dart';
import 'package:path/path.dart' as path;

class EncodeTestAsyncQuery extends IAsyncQuery<Either<StorageFailure, Unit>> {
  final TestMetadata testMetadata;

  EncodeTestAsyncQuery({
    required this.testMetadata,
  });
}

class EncodeTestAsyncQueryHandler extends IAsyncQueryHandler<
    Either<StorageFailure, Unit>, EncodeTestAsyncQuery> {
  final Logger _logger;

  EncodeTestAsyncQueryHandler(
    this._logger,
  );

  @override
  Future<Either<StorageFailure, Unit>> call(
    EncodeTestAsyncQuery command,
  ) async {
    // argument guard
    return command.testMetadata.maybeMap(
      testToEncode: (testMetadata) async {
        final testRelativePath = path.join(
          testMetadata.problemId,
          testMetadata.testId,
        );
        _logger.i(
          'Calling Encode UseCase for test $testRelativePath...',
        );

        // check if archived test file exists
        final localArchivedTestFilePath = path.join(
          testMetadata.toDir,
          '$testRelativePath.${testMetadata.archiveTypeExtension}',
        );
        final File localArchivedTestFile = File(localArchivedTestFilePath);

        if (localArchivedTestFile.existsSync()) {
          _logger.i(
            'Archived test file already exists for test $testRelativePath... Skip encoding',
          );

          return right(unit);
        }

        // check if unarchived test folder exists
        final localUnarchivedTestFolderPath = path.join(
          testMetadata.fromDir,
          testRelativePath,
        );
        final Directory localUnarchivedTestFolder = Directory(
          localUnarchivedTestFolderPath,
        );

        if (localUnarchivedTestFolder.existsSync() == false) {
          final message =
              'Unarchived test folder not found for test $testRelativePath';
          _logger.e(message);

          return left(
            StorageFailure.localTestNotFound(
              message: message,
            ),
          );
        }

        // check if unarchived test folder has a valid format
        final List<FileSystemEntity> localUnarchivedTestFolderContent =
            localUnarchivedTestFolder.listSync();
        localUnarchivedTestFolderContent.sort(
          (file1, file2) => file1.path.compareTo(file2.path),
        );

        if (localUnarchivedTestFolderContent.length != 2) {
          final message =
              'Unarchived test folder has an invalid format for test $testRelativePath: '
              'it should contain only 2 files';
          _logger.e(message);

          return left(
            StorageFailure.invalidLocalTestFormat(
              message: message,
            ),
          );
        }

        if (localUnarchivedTestFolderContent
                .elementAt(0)
                .path
                .endsWith(testMetadata.inputFilename) ==
            false) {
          final message =
              'Unarchived test folder has an invalid format for test $testRelativePath: '
              'input file name is not ${testMetadata.inputFilename}';
          _logger.e(message);

          return left(
            StorageFailure.invalidLocalTestFormat(
              message: message,
            ),
          );
        }

        if (localUnarchivedTestFolderContent
                .elementAt(1)
                .path
                .endsWith(testMetadata.outputFilename) ==
            false) {
          final message =
              'Unarchived test folder has an invalid format for test $testRelativePath: '
              'output file name is not ${testMetadata.outputFilename}';
          _logger.e(message);

          return left(
            StorageFailure.invalidLocalTestFormat(
              message: message,
            ),
          );
        }

        // archive unarchived test folder
        ZipFileEncoder().zipDirectory(
          localUnarchivedTestFolder,
          filename: localArchivedTestFilePath,
        );

        _logger.i(
          'Test $testRelativePath encoded successfully',
        );

        return right(unit);
      },
      orElse: () => left(
        StorageFailure.unexpected(
          message: 'Invalid test metadata passed to EncodeTestUseCase',
        ),
      ),
    );
  }
}
