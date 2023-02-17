// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'storage_failures.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

T _$identity<T>(T value) => value;

final _privateConstructorUsedError = UnsupportedError(
    'It seems like you constructed your class using `MyClass._()`. This constructor is only meant to be used by freezed and you are not supposed to need it nor use it.\nPlease check the documentation here for more information: https://github.com/rrousselGit/freezed#custom-getters-and-methods');

/// @nodoc
mixin _$StorageFailure {
  String get message => throw _privateConstructorUsedError;
  @optionalTypeArgs
  TResult when<TResult extends Object?>({
    required TResult Function(String message) unexpected,
    required TResult Function(String message) localTestNotFound,
    required TResult Function(String message) invalidLocalTestFormat,
    required TResult Function(String message) testSizeLimitExceeded,
  }) =>
      throw _privateConstructorUsedError;
  @optionalTypeArgs
  TResult? whenOrNull<TResult extends Object?>({
    TResult? Function(String message)? unexpected,
    TResult? Function(String message)? localTestNotFound,
    TResult? Function(String message)? invalidLocalTestFormat,
    TResult? Function(String message)? testSizeLimitExceeded,
  }) =>
      throw _privateConstructorUsedError;
  @optionalTypeArgs
  TResult maybeWhen<TResult extends Object?>({
    TResult Function(String message)? unexpected,
    TResult Function(String message)? localTestNotFound,
    TResult Function(String message)? invalidLocalTestFormat,
    TResult Function(String message)? testSizeLimitExceeded,
    required TResult orElse(),
  }) =>
      throw _privateConstructorUsedError;
  @optionalTypeArgs
  TResult map<TResult extends Object?>({
    required TResult Function(Unexpected value) unexpected,
    required TResult Function(LocalTestNotFound value) localTestNotFound,
    required TResult Function(InvalidLocalTestFormat value)
        invalidLocalTestFormat,
    required TResult Function(TestSizeLimitExceeded value)
        testSizeLimitExceeded,
  }) =>
      throw _privateConstructorUsedError;
  @optionalTypeArgs
  TResult? mapOrNull<TResult extends Object?>({
    TResult? Function(Unexpected value)? unexpected,
    TResult? Function(LocalTestNotFound value)? localTestNotFound,
    TResult? Function(InvalidLocalTestFormat value)? invalidLocalTestFormat,
    TResult? Function(TestSizeLimitExceeded value)? testSizeLimitExceeded,
  }) =>
      throw _privateConstructorUsedError;
  @optionalTypeArgs
  TResult maybeMap<TResult extends Object?>({
    TResult Function(Unexpected value)? unexpected,
    TResult Function(LocalTestNotFound value)? localTestNotFound,
    TResult Function(InvalidLocalTestFormat value)? invalidLocalTestFormat,
    TResult Function(TestSizeLimitExceeded value)? testSizeLimitExceeded,
    required TResult orElse(),
  }) =>
      throw _privateConstructorUsedError;

  @JsonKey(ignore: true)
  $StorageFailureCopyWith<StorageFailure> get copyWith =>
      throw _privateConstructorUsedError;
}

/// @nodoc
abstract class $StorageFailureCopyWith<$Res> {
  factory $StorageFailureCopyWith(
          StorageFailure value, $Res Function(StorageFailure) then) =
      _$StorageFailureCopyWithImpl<$Res, StorageFailure>;
  @useResult
  $Res call({String message});
}

/// @nodoc
class _$StorageFailureCopyWithImpl<$Res, $Val extends StorageFailure>
    implements $StorageFailureCopyWith<$Res> {
  _$StorageFailureCopyWithImpl(this._value, this._then);

  // ignore: unused_field
  final $Val _value;
  // ignore: unused_field
  final $Res Function($Val) _then;

  @pragma('vm:prefer-inline')
  @override
  $Res call({
    Object? message = null,
  }) {
    return _then(_value.copyWith(
      message: null == message
          ? _value.message
          : message // ignore: cast_nullable_to_non_nullable
              as String,
    ) as $Val);
  }
}

/// @nodoc
abstract class _$$UnexpectedCopyWith<$Res>
    implements $StorageFailureCopyWith<$Res> {
  factory _$$UnexpectedCopyWith(
          _$Unexpected value, $Res Function(_$Unexpected) then) =
      __$$UnexpectedCopyWithImpl<$Res>;
  @override
  @useResult
  $Res call({String message});
}

/// @nodoc
class __$$UnexpectedCopyWithImpl<$Res>
    extends _$StorageFailureCopyWithImpl<$Res, _$Unexpected>
    implements _$$UnexpectedCopyWith<$Res> {
  __$$UnexpectedCopyWithImpl(
      _$Unexpected _value, $Res Function(_$Unexpected) _then)
      : super(_value, _then);

  @pragma('vm:prefer-inline')
  @override
  $Res call({
    Object? message = null,
  }) {
    return _then(_$Unexpected(
      message: null == message
          ? _value.message
          : message // ignore: cast_nullable_to_non_nullable
              as String,
    ));
  }
}

/// @nodoc

class _$Unexpected implements Unexpected {
  const _$Unexpected({required this.message});

  @override
  final String message;

  @override
  String toString() {
    return 'StorageFailure.unexpected(message: $message)';
  }

  @override
  bool operator ==(dynamic other) {
    return identical(this, other) ||
        (other.runtimeType == runtimeType &&
            other is _$Unexpected &&
            (identical(other.message, message) || other.message == message));
  }

  @override
  int get hashCode => Object.hash(runtimeType, message);

  @JsonKey(ignore: true)
  @override
  @pragma('vm:prefer-inline')
  _$$UnexpectedCopyWith<_$Unexpected> get copyWith =>
      __$$UnexpectedCopyWithImpl<_$Unexpected>(this, _$identity);

  @override
  @optionalTypeArgs
  TResult when<TResult extends Object?>({
    required TResult Function(String message) unexpected,
    required TResult Function(String message) localTestNotFound,
    required TResult Function(String message) invalidLocalTestFormat,
    required TResult Function(String message) testSizeLimitExceeded,
  }) {
    return unexpected(message);
  }

  @override
  @optionalTypeArgs
  TResult? whenOrNull<TResult extends Object?>({
    TResult? Function(String message)? unexpected,
    TResult? Function(String message)? localTestNotFound,
    TResult? Function(String message)? invalidLocalTestFormat,
    TResult? Function(String message)? testSizeLimitExceeded,
  }) {
    return unexpected?.call(message);
  }

  @override
  @optionalTypeArgs
  TResult maybeWhen<TResult extends Object?>({
    TResult Function(String message)? unexpected,
    TResult Function(String message)? localTestNotFound,
    TResult Function(String message)? invalidLocalTestFormat,
    TResult Function(String message)? testSizeLimitExceeded,
    required TResult orElse(),
  }) {
    if (unexpected != null) {
      return unexpected(message);
    }
    return orElse();
  }

  @override
  @optionalTypeArgs
  TResult map<TResult extends Object?>({
    required TResult Function(Unexpected value) unexpected,
    required TResult Function(LocalTestNotFound value) localTestNotFound,
    required TResult Function(InvalidLocalTestFormat value)
        invalidLocalTestFormat,
    required TResult Function(TestSizeLimitExceeded value)
        testSizeLimitExceeded,
  }) {
    return unexpected(this);
  }

  @override
  @optionalTypeArgs
  TResult? mapOrNull<TResult extends Object?>({
    TResult? Function(Unexpected value)? unexpected,
    TResult? Function(LocalTestNotFound value)? localTestNotFound,
    TResult? Function(InvalidLocalTestFormat value)? invalidLocalTestFormat,
    TResult? Function(TestSizeLimitExceeded value)? testSizeLimitExceeded,
  }) {
    return unexpected?.call(this);
  }

  @override
  @optionalTypeArgs
  TResult maybeMap<TResult extends Object?>({
    TResult Function(Unexpected value)? unexpected,
    TResult Function(LocalTestNotFound value)? localTestNotFound,
    TResult Function(InvalidLocalTestFormat value)? invalidLocalTestFormat,
    TResult Function(TestSizeLimitExceeded value)? testSizeLimitExceeded,
    required TResult orElse(),
  }) {
    if (unexpected != null) {
      return unexpected(this);
    }
    return orElse();
  }
}

abstract class Unexpected implements StorageFailure {
  const factory Unexpected({required final String message}) = _$Unexpected;

  @override
  String get message;
  @override
  @JsonKey(ignore: true)
  _$$UnexpectedCopyWith<_$Unexpected> get copyWith =>
      throw _privateConstructorUsedError;
}

/// @nodoc
abstract class _$$LocalTestNotFoundCopyWith<$Res>
    implements $StorageFailureCopyWith<$Res> {
  factory _$$LocalTestNotFoundCopyWith(
          _$LocalTestNotFound value, $Res Function(_$LocalTestNotFound) then) =
      __$$LocalTestNotFoundCopyWithImpl<$Res>;
  @override
  @useResult
  $Res call({String message});
}

/// @nodoc
class __$$LocalTestNotFoundCopyWithImpl<$Res>
    extends _$StorageFailureCopyWithImpl<$Res, _$LocalTestNotFound>
    implements _$$LocalTestNotFoundCopyWith<$Res> {
  __$$LocalTestNotFoundCopyWithImpl(
      _$LocalTestNotFound _value, $Res Function(_$LocalTestNotFound) _then)
      : super(_value, _then);

  @pragma('vm:prefer-inline')
  @override
  $Res call({
    Object? message = null,
  }) {
    return _then(_$LocalTestNotFound(
      message: null == message
          ? _value.message
          : message // ignore: cast_nullable_to_non_nullable
              as String,
    ));
  }
}

/// @nodoc

class _$LocalTestNotFound implements LocalTestNotFound {
  const _$LocalTestNotFound({required this.message});

  @override
  final String message;

  @override
  String toString() {
    return 'StorageFailure.localTestNotFound(message: $message)';
  }

  @override
  bool operator ==(dynamic other) {
    return identical(this, other) ||
        (other.runtimeType == runtimeType &&
            other is _$LocalTestNotFound &&
            (identical(other.message, message) || other.message == message));
  }

  @override
  int get hashCode => Object.hash(runtimeType, message);

  @JsonKey(ignore: true)
  @override
  @pragma('vm:prefer-inline')
  _$$LocalTestNotFoundCopyWith<_$LocalTestNotFound> get copyWith =>
      __$$LocalTestNotFoundCopyWithImpl<_$LocalTestNotFound>(this, _$identity);

  @override
  @optionalTypeArgs
  TResult when<TResult extends Object?>({
    required TResult Function(String message) unexpected,
    required TResult Function(String message) localTestNotFound,
    required TResult Function(String message) invalidLocalTestFormat,
    required TResult Function(String message) testSizeLimitExceeded,
  }) {
    return localTestNotFound(message);
  }

  @override
  @optionalTypeArgs
  TResult? whenOrNull<TResult extends Object?>({
    TResult? Function(String message)? unexpected,
    TResult? Function(String message)? localTestNotFound,
    TResult? Function(String message)? invalidLocalTestFormat,
    TResult? Function(String message)? testSizeLimitExceeded,
  }) {
    return localTestNotFound?.call(message);
  }

  @override
  @optionalTypeArgs
  TResult maybeWhen<TResult extends Object?>({
    TResult Function(String message)? unexpected,
    TResult Function(String message)? localTestNotFound,
    TResult Function(String message)? invalidLocalTestFormat,
    TResult Function(String message)? testSizeLimitExceeded,
    required TResult orElse(),
  }) {
    if (localTestNotFound != null) {
      return localTestNotFound(message);
    }
    return orElse();
  }

  @override
  @optionalTypeArgs
  TResult map<TResult extends Object?>({
    required TResult Function(Unexpected value) unexpected,
    required TResult Function(LocalTestNotFound value) localTestNotFound,
    required TResult Function(InvalidLocalTestFormat value)
        invalidLocalTestFormat,
    required TResult Function(TestSizeLimitExceeded value)
        testSizeLimitExceeded,
  }) {
    return localTestNotFound(this);
  }

  @override
  @optionalTypeArgs
  TResult? mapOrNull<TResult extends Object?>({
    TResult? Function(Unexpected value)? unexpected,
    TResult? Function(LocalTestNotFound value)? localTestNotFound,
    TResult? Function(InvalidLocalTestFormat value)? invalidLocalTestFormat,
    TResult? Function(TestSizeLimitExceeded value)? testSizeLimitExceeded,
  }) {
    return localTestNotFound?.call(this);
  }

  @override
  @optionalTypeArgs
  TResult maybeMap<TResult extends Object?>({
    TResult Function(Unexpected value)? unexpected,
    TResult Function(LocalTestNotFound value)? localTestNotFound,
    TResult Function(InvalidLocalTestFormat value)? invalidLocalTestFormat,
    TResult Function(TestSizeLimitExceeded value)? testSizeLimitExceeded,
    required TResult orElse(),
  }) {
    if (localTestNotFound != null) {
      return localTestNotFound(this);
    }
    return orElse();
  }
}

abstract class LocalTestNotFound implements StorageFailure {
  const factory LocalTestNotFound({required final String message}) =
      _$LocalTestNotFound;

  @override
  String get message;
  @override
  @JsonKey(ignore: true)
  _$$LocalTestNotFoundCopyWith<_$LocalTestNotFound> get copyWith =>
      throw _privateConstructorUsedError;
}

/// @nodoc
abstract class _$$InvalidLocalTestFormatCopyWith<$Res>
    implements $StorageFailureCopyWith<$Res> {
  factory _$$InvalidLocalTestFormatCopyWith(_$InvalidLocalTestFormat value,
          $Res Function(_$InvalidLocalTestFormat) then) =
      __$$InvalidLocalTestFormatCopyWithImpl<$Res>;
  @override
  @useResult
  $Res call({String message});
}

/// @nodoc
class __$$InvalidLocalTestFormatCopyWithImpl<$Res>
    extends _$StorageFailureCopyWithImpl<$Res, _$InvalidLocalTestFormat>
    implements _$$InvalidLocalTestFormatCopyWith<$Res> {
  __$$InvalidLocalTestFormatCopyWithImpl(_$InvalidLocalTestFormat _value,
      $Res Function(_$InvalidLocalTestFormat) _then)
      : super(_value, _then);

  @pragma('vm:prefer-inline')
  @override
  $Res call({
    Object? message = null,
  }) {
    return _then(_$InvalidLocalTestFormat(
      message: null == message
          ? _value.message
          : message // ignore: cast_nullable_to_non_nullable
              as String,
    ));
  }
}

/// @nodoc

class _$InvalidLocalTestFormat implements InvalidLocalTestFormat {
  const _$InvalidLocalTestFormat({required this.message});

  @override
  final String message;

  @override
  String toString() {
    return 'StorageFailure.invalidLocalTestFormat(message: $message)';
  }

  @override
  bool operator ==(dynamic other) {
    return identical(this, other) ||
        (other.runtimeType == runtimeType &&
            other is _$InvalidLocalTestFormat &&
            (identical(other.message, message) || other.message == message));
  }

  @override
  int get hashCode => Object.hash(runtimeType, message);

  @JsonKey(ignore: true)
  @override
  @pragma('vm:prefer-inline')
  _$$InvalidLocalTestFormatCopyWith<_$InvalidLocalTestFormat> get copyWith =>
      __$$InvalidLocalTestFormatCopyWithImpl<_$InvalidLocalTestFormat>(
          this, _$identity);

  @override
  @optionalTypeArgs
  TResult when<TResult extends Object?>({
    required TResult Function(String message) unexpected,
    required TResult Function(String message) localTestNotFound,
    required TResult Function(String message) invalidLocalTestFormat,
    required TResult Function(String message) testSizeLimitExceeded,
  }) {
    return invalidLocalTestFormat(message);
  }

  @override
  @optionalTypeArgs
  TResult? whenOrNull<TResult extends Object?>({
    TResult? Function(String message)? unexpected,
    TResult? Function(String message)? localTestNotFound,
    TResult? Function(String message)? invalidLocalTestFormat,
    TResult? Function(String message)? testSizeLimitExceeded,
  }) {
    return invalidLocalTestFormat?.call(message);
  }

  @override
  @optionalTypeArgs
  TResult maybeWhen<TResult extends Object?>({
    TResult Function(String message)? unexpected,
    TResult Function(String message)? localTestNotFound,
    TResult Function(String message)? invalidLocalTestFormat,
    TResult Function(String message)? testSizeLimitExceeded,
    required TResult orElse(),
  }) {
    if (invalidLocalTestFormat != null) {
      return invalidLocalTestFormat(message);
    }
    return orElse();
  }

  @override
  @optionalTypeArgs
  TResult map<TResult extends Object?>({
    required TResult Function(Unexpected value) unexpected,
    required TResult Function(LocalTestNotFound value) localTestNotFound,
    required TResult Function(InvalidLocalTestFormat value)
        invalidLocalTestFormat,
    required TResult Function(TestSizeLimitExceeded value)
        testSizeLimitExceeded,
  }) {
    return invalidLocalTestFormat(this);
  }

  @override
  @optionalTypeArgs
  TResult? mapOrNull<TResult extends Object?>({
    TResult? Function(Unexpected value)? unexpected,
    TResult? Function(LocalTestNotFound value)? localTestNotFound,
    TResult? Function(InvalidLocalTestFormat value)? invalidLocalTestFormat,
    TResult? Function(TestSizeLimitExceeded value)? testSizeLimitExceeded,
  }) {
    return invalidLocalTestFormat?.call(this);
  }

  @override
  @optionalTypeArgs
  TResult maybeMap<TResult extends Object?>({
    TResult Function(Unexpected value)? unexpected,
    TResult Function(LocalTestNotFound value)? localTestNotFound,
    TResult Function(InvalidLocalTestFormat value)? invalidLocalTestFormat,
    TResult Function(TestSizeLimitExceeded value)? testSizeLimitExceeded,
    required TResult orElse(),
  }) {
    if (invalidLocalTestFormat != null) {
      return invalidLocalTestFormat(this);
    }
    return orElse();
  }
}

abstract class InvalidLocalTestFormat implements StorageFailure {
  const factory InvalidLocalTestFormat({required final String message}) =
      _$InvalidLocalTestFormat;

  @override
  String get message;
  @override
  @JsonKey(ignore: true)
  _$$InvalidLocalTestFormatCopyWith<_$InvalidLocalTestFormat> get copyWith =>
      throw _privateConstructorUsedError;
}

/// @nodoc
abstract class _$$TestSizeLimitExceededCopyWith<$Res>
    implements $StorageFailureCopyWith<$Res> {
  factory _$$TestSizeLimitExceededCopyWith(_$TestSizeLimitExceeded value,
          $Res Function(_$TestSizeLimitExceeded) then) =
      __$$TestSizeLimitExceededCopyWithImpl<$Res>;
  @override
  @useResult
  $Res call({String message});
}

/// @nodoc
class __$$TestSizeLimitExceededCopyWithImpl<$Res>
    extends _$StorageFailureCopyWithImpl<$Res, _$TestSizeLimitExceeded>
    implements _$$TestSizeLimitExceededCopyWith<$Res> {
  __$$TestSizeLimitExceededCopyWithImpl(_$TestSizeLimitExceeded _value,
      $Res Function(_$TestSizeLimitExceeded) _then)
      : super(_value, _then);

  @pragma('vm:prefer-inline')
  @override
  $Res call({
    Object? message = null,
  }) {
    return _then(_$TestSizeLimitExceeded(
      message: null == message
          ? _value.message
          : message // ignore: cast_nullable_to_non_nullable
              as String,
    ));
  }
}

/// @nodoc

class _$TestSizeLimitExceeded implements TestSizeLimitExceeded {
  const _$TestSizeLimitExceeded({required this.message});

  @override
  final String message;

  @override
  String toString() {
    return 'StorageFailure.testSizeLimitExceeded(message: $message)';
  }

  @override
  bool operator ==(dynamic other) {
    return identical(this, other) ||
        (other.runtimeType == runtimeType &&
            other is _$TestSizeLimitExceeded &&
            (identical(other.message, message) || other.message == message));
  }

  @override
  int get hashCode => Object.hash(runtimeType, message);

  @JsonKey(ignore: true)
  @override
  @pragma('vm:prefer-inline')
  _$$TestSizeLimitExceededCopyWith<_$TestSizeLimitExceeded> get copyWith =>
      __$$TestSizeLimitExceededCopyWithImpl<_$TestSizeLimitExceeded>(
          this, _$identity);

  @override
  @optionalTypeArgs
  TResult when<TResult extends Object?>({
    required TResult Function(String message) unexpected,
    required TResult Function(String message) localTestNotFound,
    required TResult Function(String message) invalidLocalTestFormat,
    required TResult Function(String message) testSizeLimitExceeded,
  }) {
    return testSizeLimitExceeded(message);
  }

  @override
  @optionalTypeArgs
  TResult? whenOrNull<TResult extends Object?>({
    TResult? Function(String message)? unexpected,
    TResult? Function(String message)? localTestNotFound,
    TResult? Function(String message)? invalidLocalTestFormat,
    TResult? Function(String message)? testSizeLimitExceeded,
  }) {
    return testSizeLimitExceeded?.call(message);
  }

  @override
  @optionalTypeArgs
  TResult maybeWhen<TResult extends Object?>({
    TResult Function(String message)? unexpected,
    TResult Function(String message)? localTestNotFound,
    TResult Function(String message)? invalidLocalTestFormat,
    TResult Function(String message)? testSizeLimitExceeded,
    required TResult orElse(),
  }) {
    if (testSizeLimitExceeded != null) {
      return testSizeLimitExceeded(message);
    }
    return orElse();
  }

  @override
  @optionalTypeArgs
  TResult map<TResult extends Object?>({
    required TResult Function(Unexpected value) unexpected,
    required TResult Function(LocalTestNotFound value) localTestNotFound,
    required TResult Function(InvalidLocalTestFormat value)
        invalidLocalTestFormat,
    required TResult Function(TestSizeLimitExceeded value)
        testSizeLimitExceeded,
  }) {
    return testSizeLimitExceeded(this);
  }

  @override
  @optionalTypeArgs
  TResult? mapOrNull<TResult extends Object?>({
    TResult? Function(Unexpected value)? unexpected,
    TResult? Function(LocalTestNotFound value)? localTestNotFound,
    TResult? Function(InvalidLocalTestFormat value)? invalidLocalTestFormat,
    TResult? Function(TestSizeLimitExceeded value)? testSizeLimitExceeded,
  }) {
    return testSizeLimitExceeded?.call(this);
  }

  @override
  @optionalTypeArgs
  TResult maybeMap<TResult extends Object?>({
    TResult Function(Unexpected value)? unexpected,
    TResult Function(LocalTestNotFound value)? localTestNotFound,
    TResult Function(InvalidLocalTestFormat value)? invalidLocalTestFormat,
    TResult Function(TestSizeLimitExceeded value)? testSizeLimitExceeded,
    required TResult orElse(),
  }) {
    if (testSizeLimitExceeded != null) {
      return testSizeLimitExceeded(this);
    }
    return orElse();
  }
}

abstract class TestSizeLimitExceeded implements StorageFailure {
  const factory TestSizeLimitExceeded({required final String message}) =
      _$TestSizeLimitExceeded;

  @override
  String get message;
  @override
  @JsonKey(ignore: true)
  _$$TestSizeLimitExceededCopyWith<_$TestSizeLimitExceeded> get copyWith =>
      throw _privateConstructorUsedError;
}
