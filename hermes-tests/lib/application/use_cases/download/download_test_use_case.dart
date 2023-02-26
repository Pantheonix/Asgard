import 'dart:io';

import 'package:cqrs_mediator/cqrs_mediator.dart';
import 'package:dartz/dartz.dart';
import 'package:hermes_tests/domain/entities/test_metadata.dart';
import 'package:hermes_tests/domain/exceptions/storage_failures.dart';
import 'package:hermes_tests/domain/interfaces/i_test_repository.dart';
import 'package:logger/logger.dart';
import 'package:path/path.dart' as path;

class DownloadTestAsyncQuery extends IAsyncQuery<Either<StorageFailure, Unit>> {
  final TestMetadata testMetadata;

  DownloadTestAsyncQuery({
    required this.testMetadata,
  });
}

class DownloadTestAsyncQueryHandler extends IAsyncQueryHandler<
    Either<StorageFailure, Unit>, DownloadTestAsyncQuery> {
  final ITestRepository _testRepository;
  final Logger _logger;

  DownloadTestAsyncQueryHandler(
    this._testRepository,
    this._logger,
  );

  @override
  Future<Either<StorageFailure, Unit>> call(
    DownloadTestAsyncQuery command,
  ) async {
    // argument guard
    return command.testMetadata.maybeMap(
      testToDownload: (testMetadata) async {
        final testRelativePath = path.join(
          testMetadata.problemId,
          testMetadata.testId,
        );
        _logger.i(
          'Calling Download UseCase for test $testRelativePath...',
        );

        // check if requested text exists on disk
        final localTestRootFolderPath = path.join(
          testMetadata.toDir,
          testRelativePath,
        );
        final Directory localTestRootFolder = Directory(
          localTestRootFolderPath,
        );

        if (localTestRootFolder.existsSync()) {
          // check if input file exists
          final localInputFilePath = path.join(
            testMetadata.toDir,
            testRelativePath,
            testMetadata.inputFilename,
          );
          final File localInputFile = File(localInputFilePath);

          if (localInputFile.existsSync() == false) {
            final message = 'Input file not found for test $testRelativePath';
            _logger.e(message);

            return left(
              StorageFailure.localTestNotFound(
                message: message,
              ),
            );
          }

          // check if output file exists
          final localOutputFilePath = path.join(
            testMetadata.toDir,
            testRelativePath,
            testMetadata.outputFilename,
          );
          final File localOutputFile = File(localOutputFilePath);

          if (localOutputFile.existsSync() == false) {
            final message = 'Output file not found for test $testRelativePath';
            _logger.e(message);

            return left(
              StorageFailure.localTestNotFound(
                message: message,
              ),
            );
          }

          _logger.i(
            'Test $testRelativePath already exists on disk. Skipping download...',
          );

          return right(unit);
        }

        final downloadResponse = await _testRepository.download(testMetadata);

        return downloadResponse.fold(
          (failure) {
            _logger.e(
              'Failed to download test $testRelativePath',
            );

            return left(failure);
          },
          (unit) {
            _logger.i(
              'Test $testRelativePath downloaded.',
            );

            return right(unit);
          },
        );
      },
      orElse: () => left(
        StorageFailure.unexpected(
          message: 'Invalid test metadata passed to DownloadTestUseCase',
        ),
      ),
    );
  }
}
