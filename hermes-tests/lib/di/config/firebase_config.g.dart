// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'firebase_config.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

_$_FirebaseConfig _$$_FirebaseConfigFromJson(Map<String, dynamic> json) =>
    _$_FirebaseConfig(
      apiKey: json['apiKey'] as String,
      authDomain: json['authDomain'] as String,
      projectId: json['projectId'] as String,
      storageBucket: json['storageBucket'] as String,
      messageSenderId: json['messageSenderId'] as String,
      appId: json['appId'] as String,
      measurementId: json['measurementId'] as String,
    );

Map<String, dynamic> _$$_FirebaseConfigToJson(_$_FirebaseConfig instance) =>
    <String, dynamic>{
      'apiKey': instance.apiKey,
      'authDomain': instance.authDomain,
      'projectId': instance.projectId,
      'storageBucket': instance.storageBucket,
      'messageSenderId': instance.messageSenderId,
      'appId': instance.appId,
      'measurementId': instance.measurementId,
    };
