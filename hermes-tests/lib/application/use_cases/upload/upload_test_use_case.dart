import 'dart:io';

import 'package:cqrs_mediator/cqrs_mediator.dart';
import 'package:dartz/dartz.dart';
import 'package:hermes_tests/domain/entities/test_metadata.dart';
import 'package:hermes_tests/domain/exceptions/storage_failures.dart';
import 'package:hermes_tests/domain/interfaces/i_test_repository.dart';
import 'package:logger/logger.dart';
import 'package:path/path.dart' as path;

class UploadTestAsyncQuery extends IAsyncQuery<Either<StorageFailure, Unit>> {
  final TestMetadata testMetadata;

  UploadTestAsyncQuery({
    required this.testMetadata,
  });
}

class UploadTestAsyncQueryHandler extends IAsyncQueryHandler<
    Either<StorageFailure, Unit>, UploadTestAsyncQuery> {
  final ITestRepository _testRepository;
  final Logger _logger;

  UploadTestAsyncQueryHandler(
    this._testRepository,
    this._logger,
  );

  @override
  Future<Either<StorageFailure, Unit>> call(
    UploadTestAsyncQuery command,
  ) async {
    return command.testMetadata.maybeMap(
      testToUpload: (testMetadata) async {
        final testRelativePath = path.join(
          testMetadata.problemId,
          testMetadata.testId,
        );
        _logger.i(
          'Calling Upload UseCase for test $testRelativePath...',
        );

        // check if input file exists
        final localInputFilePath = path.join(
          testMetadata.fromDir,
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
          testMetadata.fromDir,
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

        final uploadResponse = await _testRepository.upload(testMetadata);

        return uploadResponse.fold(
          (failure) {
            _logger.e(
              'Test $testRelativePath upload failed: $failure',
            );
            return left(failure);
          },
          (unit) {
            _logger.i(
              'Test $testRelativePath uploaded successfully',
            );
            return right(unit);
          },
        );
      },
      orElse: () => left(
        StorageFailure.unexpected(
          message: 'Invalid test metadata passed to UploadTestUseCase',
        ),
      ),
    );
  }
}
