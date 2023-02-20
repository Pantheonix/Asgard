// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'server_config.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

_$_ServerConfig _$$_ServerConfigFromJson(Map<String, dynamic> json) =>
    _$_ServerConfig(
      host: json['host'] as String,
      port: json['port'] as int,
      timeoutInSeconds: json['timeoutInSeconds'] as int,
      tempArchivedTestLocalPath: json['tempArchivedTestLocalPath'] as String,
      tempUnarchivedTestLocalPath:
          json['tempUnarchivedTestLocalPath'] as String,
      tempTestRemotePath: json['tempTestRemotePath'] as String,
      testMaxSizeInBytes: json['testMaxSizeInBytes'] as int,
      logOutputFilePath: json['logOutputFilePath'] as String,
    );

Map<String, dynamic> _$$_ServerConfigToJson(_$_ServerConfig instance) =>
    <String, dynamic>{
      'host': instance.host,
      'port': instance.port,
      'timeoutInSeconds': instance.timeoutInSeconds,
      'tempArchivedTestLocalPath': instance.tempArchivedTestLocalPath,
      'tempUnarchivedTestLocalPath': instance.tempUnarchivedTestLocalPath,
      'tempTestRemotePath': instance.tempTestRemotePath,
      'testMaxSizeInBytes': instance.testMaxSizeInBytes,
      'logOutputFilePath': instance.logOutputFilePath,
    };
