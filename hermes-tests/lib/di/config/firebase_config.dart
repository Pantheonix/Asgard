import 'package:freezed_annotation/freezed_annotation.dart';

part 'firebase_config.freezed.dart';
part 'firebase_config.g.dart';

@freezed
class FirebaseConfig with _$FirebaseConfig {
  const factory FirebaseConfig({
    required String apiKey,
    required String authDomain,
    required String projectId,
    required String storageBucket,
    required String messageSenderId,
    required String appId,
    required String measurementId,
  }) = _FirebaseConfig;

  factory FirebaseConfig.fromJson(Map<String, dynamic> json) =>
      _$FirebaseConfigFromJson(json);
}
