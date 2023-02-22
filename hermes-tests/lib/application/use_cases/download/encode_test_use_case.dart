import 'dart:io';

import 'package:archive/archive_io.dart';
import 'package:cqrs_mediator/cqrs_mediator.dart';
import 'package:dartz/dartz.dart';
import 'package:hermes_tests/domain/entities/test_metadata.dart';
import 'package:hermes_tests/domain/exceptions/storage_failures.dart';
import 'package:logger/logger.dart';

class EncodeTestAsyncQuery
    extends IAsyncQuery<Either<StorageFailure, TestMetadata>> {
  final TestMetadata testMetadata;

  EncodeTestAsyncQuery({
    required this.testMetadata,
  });
}

class EncodeTestAsyncQueryHandler extends IAsyncQueryHandler<
    Either<StorageFailure, TestMetadata>, EncodeTestAsyncQuery> {
  final Logger _logger;

  EncodeTestAsyncQueryHandler(
    this._logger,
  );

  @override
  Future<Either<StorageFailure, TestMetadata>> call(
    EncodeTestAsyncQuery command,
  ) async {
    _logger.i(
      'Calling Encode UseCase for test ${command.testMetadata.testRelativePath}...',
    );

    // check if archived test file exists
    final File localArchivedTestFile = File(
      '${command.testMetadata.destTestRootFolder}/${command.testMetadata.archivedTestRelativePath}',
    );

    if (localArchivedTestFile.existsSync()) {
      _logger.i(
        'Archived test file already exists for test ${command.testMetadata.testRelativePath}... Skip encoding',
      );

      return Future.value(
        right(
          command.testMetadata.copyWith(
            srcTestRootFolder: command.testMetadata.destTestRootFolder,
          ),
        ),
      );
    }

    // check if unarchived test folder exists
    final Directory localUnarchivedTestFolder = Directory(
      '${command.testMetadata.srcTestRootFolder}/${command.testMetadata.testRelativePath}',
    );
    if (localUnarchivedTestFolder.existsSync() == false) {
      final message =
          'Unarchived test folder not found for test ${command.testMetadata.testRelativePath}';
      _logger.e(message);

      return Future.value(
        left(
          StorageFailure.localTestNotFound(
            message: message,
          ),
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
          'Unarchived test folder has an invalid format for test ${command.testMetadata.testRelativePath}: it should contain only 2 files';
      _logger.e(message);

      return Future.value(
        left(
          StorageFailure.invalidLocalTestFormat(
            message: message,
          ),
        ),
      );
    }

    if (localUnarchivedTestFolderContent
            .elementAt(0)
            .path
            .endsWith(command.testMetadata.inputFileName) ==
        false) {
      final message =
          'Unarchived test folder has an invalid format for test ${command.testMetadata.testRelativePath}: input file name is not ${command.testMetadata.inputFileName}';
      _logger.e(message);

      return Future.value(
        left(
          StorageFailure.invalidLocalTestFormat(
            message: message,
          ),
        ),
      );
    }

    if (localUnarchivedTestFolderContent
            .elementAt(1)
            .path
            .endsWith(command.testMetadata.outputFileName) ==
        false) {
      final message =
          'Unarchived test folder has an invalid format for test ${command.testMetadata.testRelativePath}: output file name is not ${command.testMetadata.outputFileName}';
      _logger.e(message);

      return Future.value(
        left(
          StorageFailure.invalidLocalTestFormat(
            message: message,
          ),
        ),
      );
    }

    // archive unarchived test folder
    ZipFileEncoder().zipDirectory(
      localUnarchivedTestFolder,
      filename: command.testMetadata.archivedTestPath,
    );

    _logger.i(
      'Test ${command.testMetadata.testRelativePath} encoded successfully',
    );

    return Future.value(
      right(
        command.testMetadata.copyWith(
          srcTestRootFolder: command.testMetadata.destTestRootFolder,
        ),
      ),
    );
  }
}
