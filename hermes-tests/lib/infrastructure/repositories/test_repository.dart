import 'dart:io';

import 'package:dartz/dartz.dart';
import 'package:firebase_dart/firebase_dart.dart';
import 'package:hermes_tests/domain/entities/test_metadata.dart';
import 'package:hermes_tests/domain/exceptions/storage_failures.dart';
import 'package:hermes_tests/domain/interfaces/i_test_repository.dart';
import 'package:injectable/injectable.dart';
import 'package:path/path.dart' as path;

@LazySingleton(as: ITestRepository)
class TestRepository implements ITestRepository {
  final FirebaseStorage _storage;

  TestRepository(
    this._storage,
  );

  @override
  Future<Either<StorageFailure, Unit>> upload(
    TestMetadata testMetadata,
  ) async {
    return await testMetadata.maybeMap(
      testToUpload: (testMetadata) async {
        try {
          final localInputFilePath = path.join(
            testMetadata.fromDir,
            testMetadata.problemId,
            testMetadata.testId,
            testMetadata.inputFilename,
          );
          final File localInputFile = File(localInputFilePath);

          final remoteInputFilePath = path.join(
            testMetadata.toDir,
            testMetadata.problemId,
            testMetadata.testId,
            testMetadata.inputFilename,
          );
          await _storage
              .ref(remoteInputFilePath)
              .putData(localInputFile.readAsBytesSync());

          final localOutputFilePath = path.join(
            testMetadata.fromDir,
            testMetadata.problemId,
            testMetadata.testId,
            testMetadata.outputFilename,
          );
          final File localOutputFile = File(localOutputFilePath);

          final remoteOutputFilePath = path.join(
            testMetadata.toDir,
            testMetadata.problemId,
            testMetadata.testId,
            testMetadata.outputFilename,
          );
          await _storage
              .ref(remoteOutputFilePath)
              .putData(localOutputFile.readAsBytesSync());

          return right(unit);
        } catch (e) {
          return left(
            StorageFailure.unexpected(
              message: e.toString(),
            ),
          );
        }
      },
      orElse: () => left(
        StorageFailure.unexpected(
          message: 'Invalid test metadata passed to repository upload method',
        ),
      ),
    );
  }

  @override
  Future<Either<StorageFailure, Unit>> download(
    TestMetadata testMetadata,
  ) async {
    return await testMetadata.maybeMap(
      testToDownload: (testMetadata) async {
        try {
          final localTestRootFolderPath = path.join(
            testMetadata.toDir,
            testMetadata.problemId,
            testMetadata.testId,
          );
          final Directory localTestRootFolder = Directory(
            localTestRootFolderPath,
          );
          localTestRootFolder.createSync(
            recursive: true,
          );

          final localInputFilePath = path.join(
            testMetadata.toDir,
            testMetadata.problemId,
            testMetadata.testId,
            testMetadata.inputFilename,
          );
          final File localInputFile = File(localInputFilePath);
          localInputFile.createSync(
            recursive: true,
          );

          final remoteInputFilePath = path.join(
            testMetadata.fromDir,
            testMetadata.problemId,
            testMetadata.testId,
            testMetadata.inputFilename,
          );
          await localInputFile.writeAsBytes(
            (await _storage.ref(remoteInputFilePath).getData()) as List<int>,
          );

          final localOutputFilePath = path.join(
            testMetadata.toDir,
            testMetadata.problemId,
            testMetadata.testId,
            testMetadata.outputFilename,
          );
          final File localOutputFile = File(localOutputFilePath);
          localOutputFile.createSync(
            recursive: true,
          );

          final remoteOutputFilePath = path.join(
            testMetadata.fromDir,
            testMetadata.problemId,
            testMetadata.testId,
            testMetadata.outputFilename,
          );
          await localOutputFile.writeAsBytes(
            (await _storage.ref(remoteOutputFilePath).getData()) as List<int>,
          );

          return right(unit);
        } catch (e) {
          return left(
            StorageFailure.unexpected(
              message: e.toString(),
            ),
          );
        }
      },
      orElse: () => left(
        StorageFailure.unexpected(
          message: 'Invalid test metadata passed to repository download method',
        ),
      ),
    );
  }

  @override
  Future<Either<StorageFailure, Unit>> delete(TestMetadata testMetadata) async {
    return await testMetadata.maybeMap(
      testToDelete: (testMetadata) async {
        try {
          final remoteInputFilePath = path.join(
            testMetadata.fromDir,
            testMetadata.problemId,
            testMetadata.testId,
            testMetadata.inputFilename,
          );
          await _storage.ref(remoteInputFilePath).delete();

          final remoteOutputFilePath = path.join(
            testMetadata.fromDir,
            testMetadata.problemId,
            testMetadata.testId,
            testMetadata.outputFilename,
          );
          await _storage.ref(remoteOutputFilePath).delete();

          return right(unit);
        } catch (e) {
          return left(
            StorageFailure.unexpected(
              message: e.toString(),
            ),
          );
        }
      }, 
      orElse: () {
        return left(
          StorageFailure.unexpected(
            message: 'Invalid test metadata passed to repository delete method',
          ),
        );
      }
    );
  }

  @override
  Future<Either<StorageFailure, Unit>> getDownloadLinkForTest(
      TestMetadata testMetadata) {
    // TODO: implement getDownloadLinkForTest
    throw UnimplementedError();
  }
}
