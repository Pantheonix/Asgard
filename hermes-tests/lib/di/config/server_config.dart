import 'package:freezed_annotation/freezed_annotation.dart';

part 'server_config.freezed.dart';
part 'server_config.g.dart';

@freezed
class ServerConfig with _$ServerConfig {
  const factory ServerConfig({
    required String host,
    required int port,
    required int timeoutInSeconds,
    required String tempArchivedTestLocalPath,
    required String tempUnarchivedTestLocalPath,
    required String tempTestRemotePath,
    required int testMaxSizeInBytes,
  }) = _ServerConfig;

  factory ServerConfig.fromJson(Map<String, dynamic> json) =>
      _$ServerConfigFromJson(json);
}
