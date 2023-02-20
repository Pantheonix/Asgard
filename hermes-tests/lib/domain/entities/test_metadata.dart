import 'package:freezed_annotation/freezed_annotation.dart';

part 'test_metadata.freezed.dart';

@freezed
class TestMetadata with _$TestMetadata {
  const factory TestMetadata({
    required String problemId,
    required String testId,
    required String srcTestRootFolder,
    required String destTestRootFolder,
    @Default("input.txt") String inputFileName,
    @Default("output.txt") String outputFileName,
  }) = _TestMetadata;

  const TestMetadata._();

  String get testRelativePath => '$problemId/$testId';
  String get archivedTestRelativePath => '$problemId/$testId.zip';

  String get archivedTestPath => '$srcTestRootFolder/$archivedTestRelativePath';
  String get unarchivedTestPath => '$destTestRootFolder/$testRelativePath';

  String get srcTestInputPath =>
      '$srcTestRootFolder/$testRelativePath/$inputFileName';
  String get srcTestOutputPath =>
      '$srcTestRootFolder/$testRelativePath/$outputFileName';

  String get destTestInputPath =>
      '$destTestRootFolder/$testRelativePath/$inputFileName';
  String get destTestOutputPath =>
      '$destTestRootFolder/$testRelativePath/$outputFileName';
}
