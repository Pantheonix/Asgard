import 'package:hermes_tests/domain/entities/test_metadata.dart';

abstract class ITestRepository {
  Future<void> upload(TestMetadata testMetadata);
  Future<void> download(TestMetadata testMetadata);
}
