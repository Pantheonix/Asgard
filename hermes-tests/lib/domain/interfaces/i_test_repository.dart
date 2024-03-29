import 'package:dartz/dartz.dart';
import 'package:hermes_tests/api/core/hermes.pb.dart';
import 'package:hermes_tests/domain/entities/test_metadata.dart';
import 'package:hermes_tests/domain/exceptions/storage_failures.dart';

abstract class ITestRepository {
  Future<Either<StorageFailure, Unit>> upload(TestMetadata testMetadata);
  Future<Either<StorageFailure, Unit>> download(TestMetadata testMetadata);
  Future<Either<StorageFailure, Unit>> delete(TestMetadata testMetadata);
  Future<Either<StorageFailure, GetDownloadLinkForTestResponse>> getDownloadLinkForTest(
      TestMetadata testMetadata);
}
