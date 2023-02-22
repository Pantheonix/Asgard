import 'dart:io';

import 'package:cqrs_mediator/cqrs_mediator.dart';
import 'package:dartz/dartz.dart';
import 'package:hermes_tests/domain/entities/test_metadata.dart';
import 'package:hermes_tests/domain/exceptions/storage_failures.dart';
import 'package:hermes_tests/domain/interfaces/i_test_repository.dart';
import 'package:logger/logger.dart';

class DownloadTestAsyncQuery
    extends IAsyncQuery<Either<StorageFailure, TestMetadata>> {
  final TestMetadata testMetadata;
  final String destTestRootFolderForDownloadedTest;

  DownloadTestAsyncQuery({
    required this.testMetadata,
    required this.destTestRootFolderForDownloadedTest,
  });
}

class DownloadTestAsyncQueryHandler extends IAsyncQueryHandler<
    Either<StorageFailure, TestMetadata>, DownloadTestAsyncQuery> {
  final ITestRepository _testRepository;
  final Logger _logger;

  DownloadTestAsyncQueryHandler(
    this._testRepository,
    this._logger,
  );

  @override
  Future<Either<StorageFailure, TestMetadata>> call(
    DownloadTestAsyncQuery command,
  ) async {
    _logger.i(
      'Calling Download UseCase for test ${command.testMetadata.testRelativePath}...',
    );

    // check if requested text exists on disk
    final Directory localTestRootFolder = Directory(
      '${command.testMetadata.destTestRootFolder}/${command.testMetadata.testRelativePath}',
    );

    if (localTestRootFolder.existsSync()) {
      // check if input file exists
      final File localInputFile = File(command.testMetadata.destTestInputPath);
      if (localInputFile.existsSync() == false) {
        final message =
            'Input file not found for test ${command.testMetadata.testRelativePath}';
        _logger.e(message);

        return Future.value(
          left(
            StorageFailure.localTestNotFound(
              message: message,
            ),
          ),
        );
      }

      // check if output file exists
      final File localOutputFile =
          File(command.testMetadata.destTestOutputPath);
      if (localOutputFile.existsSync() == false) {
        final message =
            'Output file not found for test ${command.testMetadata.testRelativePath}';
        _logger.e(message);

        return Future.value(
          left(
            StorageFailure.localTestNotFound(
              message: message,
            ),
          ),
        );
      }

      _logger.i(
        'Test ${command.testMetadata.testRelativePath} already exists on disk. Skipping download...',
      );

      return Future.value(
        right(
          command.testMetadata.copyWith(
            srcTestRootFolder: command.testMetadata.destTestRootFolder,
            destTestRootFolder: command.destTestRootFolderForDownloadedTest,
          ),
        ),
      );
    }

    try {
      await _testRepository.download(command.testMetadata);

      _logger.i(
        'Test ${command.testMetadata.testRelativePath} downloaded.',
      );

      return Future.value(
        right(
          command.testMetadata.copyWith(
            srcTestRootFolder: command.testMetadata.destTestRootFolder,
            destTestRootFolder: command.destTestRootFolderForDownloadedTest,
          ),
        ),
      );
    } catch (e) {
      _logger.e(
        '${e.toString()} when downloading test ${command.testMetadata.testRelativePath}',
      );

      return Future.value(
        left(
          StorageFailure.unexpected(
            message:
                'Unable to download test ${command.testMetadata.testRelativePath}',
          ),
        ),
      );
    }
  }
}
