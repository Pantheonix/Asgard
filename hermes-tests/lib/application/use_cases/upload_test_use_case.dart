import 'dart:io';

import 'package:cqrs_mediator/cqrs_mediator.dart';
import 'package:dartz/dartz.dart';
import 'package:firebase_dart/firebase_dart.dart';
import 'package:hermes_tests/domain/entities/test_metadata.dart';
import 'package:hermes_tests/domain/exceptions/storage_failures.dart';
import 'package:hermes_tests/domain/interfaces/i_test_repository.dart';

class UploadTestAsyncQuery extends IAsyncQuery<Either<StorageFailure, Unit>> {
  final TestMetadata testMetadata;

  UploadTestAsyncQuery({
    required this.testMetadata,
  });
}

class UploadTestAsyncQueryHandler extends IAsyncQueryHandler<
    Either<StorageFailure, Unit>, UploadTestAsyncQuery> {
  final ITestRepository _testRepository;

  UploadTestAsyncQueryHandler(this._testRepository);

  @override
  Future<Either<StorageFailure, Unit>> call(
    UploadTestAsyncQuery command,
  ) async {
    // check if input file exists
    final File localInputFile = File(command.testMetadata.srcTestInputPath);
    if (localInputFile.existsSync() == false) {
      return Future.value(
        left(
          StorageFailure.invalidLocalTest(
            message:
                'Input file not found for test ${command.testMetadata.testRelativePath}',
          ),
        ),
      );
    }

    // check if output file exists
    final File localOutputFile = File(command.testMetadata.srcTestOutputPath);
    if (localOutputFile.existsSync() == false) {
      return Future.value(
        left(
          StorageFailure.invalidLocalTest(
            message:
                'Output file not found for test ${command.testMetadata.testRelativePath}',
          ),
        ),
      );
    }

    try {
      await _testRepository.upload(command.testMetadata);

      return Future.value(right(unit));
    } on FirebaseException catch (e) {
      return Future.value(
        left(
          StorageFailure.unexpected(
            message:
                '${e.toString()} Unable to upload test ${command.testMetadata.testRelativePath}',
          ),
        ),
      );
    }
  }
}
