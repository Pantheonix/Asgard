import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:hermes_tests/api/core/hermes.pb.dart';

part 'test_metadata.freezed.dart';

@freezed
class TestMetadata with _$TestMetadata {
  const factory TestMetadata.testToFragment({
    required String problemId,
    required String testId,
    required String fromDir,
    required String archiveTypeExtension,
  }) = TestToFragment;

  const factory TestMetadata.testToEncode({
    required String problemId,
    required String testId,
    required String fromDir,
    required String toDir,
    required String archiveTypeExtension,
    required String inputFilename,
    required String outputFilename,
  }) = TestToEncode;

  const factory TestMetadata.testToDownload({
    required String problemId,
    required String testId,
    required String fromDir,
    required String toDir,
    required String inputFilename,
    required String outputFilename,
  }) = TestToDownload;

  const factory TestMetadata.testToDefragment({
    required String problemId,
    required String testId,
    required int testSize,
    required String toDir,
    required String archiveTypeExtension,
    required Stream<Chunk> chunkStream,
    required int maxTestSize,
  }) = TestToDefragment;

  const factory TestMetadata.testToDecode({
    required String problemId,
    required String testId,
    required String fromDir,
    required String toDir,
    required String archiveTypeExtension,
    required String inputFilename,
    required String outputFilename,
  }) = TestToDecode;

  const factory TestMetadata.testToUpload({
    required String problemId,
    required String testId,
    required String fromDir,
    required String toDir,
    required String inputFilename,
    required String outputFilename,
  }) = TestToUpload;

  const factory TestMetadata.testToDelete({
    required String problemId,
    required String testId,
    required String fromDir,
    required String inputFilename,
    required String outputFilename,
  }) = TestToDelete;

  // const factory TestMetadata.testToGetDownloadLink({
  //   required String problemId,
  //   required String testId,
  //   required String fromDir,
  //   required String toDir,
  //   required String inputFilename,
  //   required String outputFilename,
  // }) = TestToGetDownloadLink;
}
