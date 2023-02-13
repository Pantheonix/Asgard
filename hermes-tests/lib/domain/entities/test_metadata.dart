import 'package:freezed_annotation/freezed_annotation.dart';

part 'test_metadata.freezed.dart';

@freezed
class TestMetadata with _$TestMetadata {
  const factory TestMetadata({
    required String problemId,
    required String testId,
  }) = _TestMetadata;

  const TestMetadata._();

  String get testRelativePath => '$problemId/$testId';
}
