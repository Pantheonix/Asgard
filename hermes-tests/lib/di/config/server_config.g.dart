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
      tempLocalArchivedTestFolder:
          json['tempLocalArchivedTestFolder'] as String,
      tempLocalUnarchivedTestFolder:
          json['tempLocalUnarchivedTestFolder'] as String,
      remoteUnarchivedTestFolder: json['remoteUnarchivedTestFolder'] as String,
      inputFilename: json['inputFilename'] as String,
      outputFilename: json['outputFilename'] as String,
      archiveTypeExtension: json['archiveTypeExtension'] as String,
      testMaxSizeInBytes: json['testMaxSizeInBytes'] as int,
      logOutputFilePath: json['logOutputFilePath'] as String,
    );

Map<String, dynamic> _$$_ServerConfigToJson(_$_ServerConfig instance) =>
    <String, dynamic>{
      'host': instance.host,
      'port': instance.port,
      'timeoutInSeconds': instance.timeoutInSeconds,
      'tempLocalArchivedTestFolder': instance.tempLocalArchivedTestFolder,
      'tempLocalUnarchivedTestFolder': instance.tempLocalUnarchivedTestFolder,
      'remoteUnarchivedTestFolder': instance.remoteUnarchivedTestFolder,
      'inputFilename': instance.inputFilename,
      'outputFilename': instance.outputFilename,
      'archiveTypeExtension': instance.archiveTypeExtension,
      'testMaxSizeInBytes': instance.testMaxSizeInBytes,
      'logOutputFilePath': instance.logOutputFilePath,
    };
