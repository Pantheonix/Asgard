import 'dart:io';

import 'package:cqrs_mediator/cqrs_mediator.dart';
import 'package:dartz/dartz.dart';
import 'package:hermes_tests/domain/entities/test_metadata.dart';
import 'package:hermes_tests/domain/exceptions/storage_failures.dart';
import 'package:hermes_tests/domain/interfaces/i_test_repository.dart';
import 'package:logger/logger.dart';

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
    _logger.i(
      'Calling Upload UseCase for test ${command.testMetadata.testRelativePath}...',
    );

    // check if input file exists
    final File localInputFile = File(command.testMetadata.srcTestInputPath);
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
    final File localOutputFile = File(command.testMetadata.srcTestOutputPath);
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

    try {
      await _testRepository.upload(command.testMetadata);
      _logger.i(
        'Test successfully uploaded to ${command.testMetadata.destTestRootFolder}/${command.testMetadata.testRelativePath}',
      );

      return Future.value(right(unit));
    } on Exception catch (e) {
      _logger.e(e.toString());

      return Future.value(
        left(
          StorageFailure.unexpected(
            message:
                'Unable to upload test ${command.testMetadata.testRelativePath}',
          ),
        ),
      );
    }
  }
}
