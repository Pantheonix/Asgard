import 'dart:io';

import 'package:cqrs_mediator/cqrs_mediator.dart';
import 'package:dartz/dartz.dart';
import 'package:hermes_tests/domain/entities/test_metadata.dart';
import 'package:hermes_tests/domain/exceptions/storage_failures.dart';
import 'package:archive/archive_io.dart';
import 'package:logger/logger.dart';

class DecodeTestAsyncQuery
    extends IAsyncQuery<Either<StorageFailure, TestMetadata>> {
  final TestMetadata testMetadata;
  final String destTestRootFolderForUnarchivedTest;

  DecodeTestAsyncQuery({
    required this.testMetadata,
    required this.destTestRootFolderForUnarchivedTest,
  });
}

class DecodeTestAsyncQueryHandler extends IAsyncQueryHandler<
    Either<StorageFailure, TestMetadata>, DecodeTestAsyncQuery> {
  final Logger _logger;

  DecodeTestAsyncQueryHandler(
    this._logger,
  );

  @override
  Future<Either<StorageFailure, TestMetadata>> call(
    DecodeTestAsyncQuery command,
  ) async {
    _logger.i(
      'Calling Decode UseCase for test ${command.testMetadata.testRelativePath}...',
    );

    // check if archived test file exists
    final File localArchivedTestFile =
        File(command.testMetadata.archivedTestPath);
    if (localArchivedTestFile.existsSync() == false) {
      final message =
          'Archived test file not found for test ${command.testMetadata.testRelativePath}';
      _logger.e(message);

      return Future.value(
        left(
          StorageFailure.localTestNotFound(
            message: message,
          ),
        ),
      );
    }

    final archivedTestInputStream = InputFileStream(
      command.testMetadata.archivedTestPath,
    );
    final archive = ZipDecoder().decodeBuffer(archivedTestInputStream);
    extractArchiveToDisk(archive, command.testMetadata.unarchivedTestPath);

    _logger.i(
      'Test decoded and saved to ${command.testMetadata.unarchivedTestPath}',
    );

    return Future.value(
      right(
        command.testMetadata.copyWith(
          srcTestRootFolder: command.testMetadata.destTestRootFolder,
          destTestRootFolder: command.destTestRootFolderForUnarchivedTest,
        ),
      ),
    );
  }
}
