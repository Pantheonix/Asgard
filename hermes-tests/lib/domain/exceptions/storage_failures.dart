import 'package:freezed_annotation/freezed_annotation.dart';

part 'storage_failures.freezed.dart';

@freezed
class StorageFailure with _$StorageFailure {
  const factory StorageFailure.unexpected({
    required String message,
  }) = Unexpected;
  const factory StorageFailure.localTestNotFound({
    required String message,
  }) = LocalTestNotFound;
  const factory StorageFailure.invalidLocalTestFormat({
    required String message,
  }) = InvalidLocalTestFormat;
  const factory StorageFailure.testSizeLimitExceeded({
    required String message,
  }) = TestSizeLimitExceeded;
}
