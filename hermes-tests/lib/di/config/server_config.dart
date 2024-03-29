import 'package:freezed_annotation/freezed_annotation.dart';

part 'server_config.freezed.dart';
part 'server_config.g.dart';

@freezed
class ServerConfig with _$ServerConfig {
  const factory ServerConfig({
    required String host,
    required int port,
    required int timeoutInSeconds,
    required String tempLocalArchivedTestFolder,
    required String tempLocalUnarchivedTestFolder,
    required String remoteUnarchivedTestFolder,
    required String inputFilename,
    required String outputFilename,
    required String archiveTypeExtension,
    required int testMaxSizeInBytes,
    required String logOutputFilePath,
  }) = _ServerConfig;

  factory ServerConfig.fromJson(Map<String, dynamic> json) =>
      _$ServerConfigFromJson(json);
}
