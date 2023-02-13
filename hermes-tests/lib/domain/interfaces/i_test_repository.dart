import 'package:dartz/dartz.dart';
import 'package:hermes_tests/domain/entities/test_metadata.dart';
import 'package:hermes_tests/domain/exceptions/storage_failures.dart';

abstract class ITestRepository {
  Future<Either<StorageFailure, Unit>> upload(
    TestMetadata testMetadata,
    String localTestRootFolder,
    String remoteTestRootFolder,
  );
}
