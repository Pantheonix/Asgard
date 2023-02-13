import 'dart:io';

import 'package:firebase_dart/firebase_dart.dart';
import 'package:hermes_tests/domain/exceptions/storage_failures.dart';
import 'package:hermes_tests/domain/entities/test_metadata.dart';
import 'package:dartz/dartz.dart';
import 'package:hermes_tests/domain/interfaces/i_test_repository.dart';
import 'package:injectable/injectable.dart';

@LazySingleton(as: ITestRepository)
class TestRepository implements ITestRepository {
  final FirebaseStorage _storage;

  TestRepository(this._storage);

  @override
  Future<Either<StorageFailure, Unit>> upload(
    TestMetadata testMetadata,
    String localTestRootFolder,
    String remoteTestRootFolder,
  ) async {
    final String localInputFilePath =
        '$localTestRootFolder/${testMetadata.testRelativePath}/input.txt';
    final File localInputFile = File(localInputFilePath);

    final String localOutputFilePath =
        '$localTestRootFolder/${testMetadata.testRelativePath}/output.txt';
    final File localOutputFile = File(localOutputFilePath);

    // check if input file exists
    if (localInputFile.existsSync() == false) {
      return Future.value(
        left(
          StorageFailure.invalidLocalTest(
            message: 'Input file not found',
          ),
        ),
      );
    }

    // check if output file exists
    if (localOutputFile.existsSync() == false) {
      return Future.value(
        left(
          StorageFailure.invalidLocalTest(
            message: 'Output file not found',
          ),
        ),
      );
    }

    final String remoteInputFilePath =
        '$remoteTestRootFolder/${testMetadata.testRelativePath}/input.txt';
    final String remoteOutputFilePath =
        '$remoteTestRootFolder/${testMetadata.testRelativePath}/output.txt';

    try {
      await _storage
          .ref(remoteInputFilePath)
          .putData(localInputFile.readAsBytesSync());
      await _storage
          .ref(remoteOutputFilePath)
          .putData(localOutputFile.readAsBytesSync());

      return Future.value(right(unit));
    } on FirebaseException catch (e) {
      return Future.value(
        left(
          StorageFailure.unexpected(
            message:
                e.message ?? 'Unable to upload test ${testMetadata.testId}',
          ),
        ),
      );
    }
  }
}
