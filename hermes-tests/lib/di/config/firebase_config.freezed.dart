// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'firebase_config.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

T _$identity<T>(T value) => value;

final _privateConstructorUsedError = UnsupportedError(
    'It seems like you constructed your class using `MyClass._()`. This constructor is only meant to be used by freezed and you are not supposed to need it nor use it.\nPlease check the documentation here for more information: https://github.com/rrousselGit/freezed#custom-getters-and-methods');

FirebaseConfig _$FirebaseConfigFromJson(Map<String, dynamic> json) {
  return _FirebaseConfig.fromJson(json);
}

/// @nodoc
mixin _$FirebaseConfig {
  String get apiKey => throw _privateConstructorUsedError;
  String get authDomain => throw _privateConstructorUsedError;
  String get projectId => throw _privateConstructorUsedError;
  String get storageBucket => throw _privateConstructorUsedError;
  String get messageSenderId => throw _privateConstructorUsedError;
  String get appId => throw _privateConstructorUsedError;
  String get measurementId => throw _privateConstructorUsedError;

  Map<String, dynamic> toJson() => throw _privateConstructorUsedError;
  @JsonKey(ignore: true)
  $FirebaseConfigCopyWith<FirebaseConfig> get copyWith =>
      throw _privateConstructorUsedError;
}

/// @nodoc
abstract class $FirebaseConfigCopyWith<$Res> {
  factory $FirebaseConfigCopyWith(
          FirebaseConfig value, $Res Function(FirebaseConfig) then) =
      _$FirebaseConfigCopyWithImpl<$Res, FirebaseConfig>;
  @useResult
  $Res call(
      {String apiKey,
      String authDomain,
      String projectId,
      String storageBucket,
      String messageSenderId,
      String appId,
      String measurementId});
}

/// @nodoc
class _$FirebaseConfigCopyWithImpl<$Res, $Val extends FirebaseConfig>
    implements $FirebaseConfigCopyWith<$Res> {
  _$FirebaseConfigCopyWithImpl(this._value, this._then);

  // ignore: unused_field
  final $Val _value;
  // ignore: unused_field
  final $Res Function($Val) _then;

  @pragma('vm:prefer-inline')
  @override
  $Res call({
    Object? apiKey = null,
    Object? authDomain = null,
    Object? projectId = null,
    Object? storageBucket = null,
    Object? messageSenderId = null,
    Object? appId = null,
    Object? measurementId = null,
  }) {
    return _then(_value.copyWith(
      apiKey: null == apiKey
          ? _value.apiKey
          : apiKey // ignore: cast_nullable_to_non_nullable
              as String,
      authDomain: null == authDomain
          ? _value.authDomain
          : authDomain // ignore: cast_nullable_to_non_nullable
              as String,
      projectId: null == projectId
          ? _value.projectId
          : projectId // ignore: cast_nullable_to_non_nullable
              as String,
      storageBucket: null == storageBucket
          ? _value.storageBucket
          : storageBucket // ignore: cast_nullable_to_non_nullable
              as String,
      messageSenderId: null == messageSenderId
          ? _value.messageSenderId
          : messageSenderId // ignore: cast_nullable_to_non_nullable
              as String,
      appId: null == appId
          ? _value.appId
          : appId // ignore: cast_nullable_to_non_nullable
              as String,
      measurementId: null == measurementId
          ? _value.measurementId
          : measurementId // ignore: cast_nullable_to_non_nullable
              as String,
    ) as $Val);
  }
}

/// @nodoc
abstract class _$$_FirebaseConfigCopyWith<$Res>
    implements $FirebaseConfigCopyWith<$Res> {
  factory _$$_FirebaseConfigCopyWith(
          _$_FirebaseConfig value, $Res Function(_$_FirebaseConfig) then) =
      __$$_FirebaseConfigCopyWithImpl<$Res>;
  @override
  @useResult
  $Res call(
      {String apiKey,
      String authDomain,
      String projectId,
      String storageBucket,
      String messageSenderId,
      String appId,
      String measurementId});
}

/// @nodoc
class __$$_FirebaseConfigCopyWithImpl<$Res>
    extends _$FirebaseConfigCopyWithImpl<$Res, _$_FirebaseConfig>
    implements _$$_FirebaseConfigCopyWith<$Res> {
  __$$_FirebaseConfigCopyWithImpl(
      _$_FirebaseConfig _value, $Res Function(_$_FirebaseConfig) _then)
      : super(_value, _then);

  @pragma('vm:prefer-inline')
  @override
  $Res call({
    Object? apiKey = null,
    Object? authDomain = null,
    Object? projectId = null,
    Object? storageBucket = null,
    Object? messageSenderId = null,
    Object? appId = null,
    Object? measurementId = null,
  }) {
    return _then(_$_FirebaseConfig(
      apiKey: null == apiKey
          ? _value.apiKey
          : apiKey // ignore: cast_nullable_to_non_nullable
              as String,
      authDomain: null == authDomain
          ? _value.authDomain
          : authDomain // ignore: cast_nullable_to_non_nullable
              as String,
      projectId: null == projectId
          ? _value.projectId
          : projectId // ignore: cast_nullable_to_non_nullable
              as String,
      storageBucket: null == storageBucket
          ? _value.storageBucket
          : storageBucket // ignore: cast_nullable_to_non_nullable
              as String,
      messageSenderId: null == messageSenderId
          ? _value.messageSenderId
          : messageSenderId // ignore: cast_nullable_to_non_nullable
              as String,
      appId: null == appId
          ? _value.appId
          : appId // ignore: cast_nullable_to_non_nullable
              as String,
      measurementId: null == measurementId
          ? _value.measurementId
          : measurementId // ignore: cast_nullable_to_non_nullable
              as String,
    ));
  }
}

/// @nodoc
@JsonSerializable()
class _$_FirebaseConfig implements _FirebaseConfig {
  const _$_FirebaseConfig(
      {required this.apiKey,
      required this.authDomain,
      required this.projectId,
      required this.storageBucket,
      required this.messageSenderId,
      required this.appId,
      required this.measurementId});

  factory _$_FirebaseConfig.fromJson(Map<String, dynamic> json) =>
      _$$_FirebaseConfigFromJson(json);

  @override
  final String apiKey;
  @override
  final String authDomain;
  @override
  final String projectId;
  @override
  final String storageBucket;
  @override
  final String messageSenderId;
  @override
  final String appId;
  @override
  final String measurementId;

  @override
  String toString() {
    return 'FirebaseConfig(apiKey: $apiKey, authDomain: $authDomain, projectId: $projectId, storageBucket: $storageBucket, messageSenderId: $messageSenderId, appId: $appId, measurementId: $measurementId)';
  }

  @override
  bool operator ==(dynamic other) {
    return identical(this, other) ||
        (other.runtimeType == runtimeType &&
            other is _$_FirebaseConfig &&
            (identical(other.apiKey, apiKey) || other.apiKey == apiKey) &&
            (identical(other.authDomain, authDomain) ||
                other.authDomain == authDomain) &&
            (identical(other.projectId, projectId) ||
                other.projectId == projectId) &&
            (identical(other.storageBucket, storageBucket) ||
                other.storageBucket == storageBucket) &&
            (identical(other.messageSenderId, messageSenderId) ||
                other.messageSenderId == messageSenderId) &&
            (identical(other.appId, appId) || other.appId == appId) &&
            (identical(other.measurementId, measurementId) ||
                other.measurementId == measurementId));
  }

  @JsonKey(ignore: true)
  @override
  int get hashCode => Object.hash(runtimeType, apiKey, authDomain, projectId,
      storageBucket, messageSenderId, appId, measurementId);

  @JsonKey(ignore: true)
  @override
  @pragma('vm:prefer-inline')
  _$$_FirebaseConfigCopyWith<_$_FirebaseConfig> get copyWith =>
      __$$_FirebaseConfigCopyWithImpl<_$_FirebaseConfig>(this, _$identity);

  @override
  Map<String, dynamic> toJson() {
    return _$$_FirebaseConfigToJson(
      this,
    );
  }
}

abstract class _FirebaseConfig implements FirebaseConfig {
  const factory _FirebaseConfig(
      {required final String apiKey,
      required final String authDomain,
      required final String projectId,
      required final String storageBucket,
      required final String messageSenderId,
      required final String appId,
      required final String measurementId}) = _$_FirebaseConfig;

  factory _FirebaseConfig.fromJson(Map<String, dynamic> json) =
      _$_FirebaseConfig.fromJson;

  @override
  String get apiKey;
  @override
  String get authDomain;
  @override
  String get projectId;
  @override
  String get storageBucket;
  @override
  String get messageSenderId;
  @override
  String get appId;
  @override
  String get measurementId;
  @override
  @JsonKey(ignore: true)
  _$$_FirebaseConfigCopyWith<_$_FirebaseConfig> get copyWith =>
      throw _privateConstructorUsedError;
}
