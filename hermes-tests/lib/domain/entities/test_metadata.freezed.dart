// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'test_metadata.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

T _$identity<T>(T value) => value;

final _privateConstructorUsedError = UnsupportedError(
    'It seems like you constructed your class using `MyClass._()`. This constructor is only meant to be used by freezed and you are not supposed to need it nor use it.\nPlease check the documentation here for more information: https://github.com/rrousselGit/freezed#custom-getters-and-methods');

/// @nodoc
mixin _$TestMetadata {
  String get problemId => throw _privateConstructorUsedError;
  String get testId => throw _privateConstructorUsedError;
  String get srcTestRootFolder => throw _privateConstructorUsedError;
  String get destTestRootFolder => throw _privateConstructorUsedError;
  String get inputFileName => throw _privateConstructorUsedError;
  String get outputFileName => throw _privateConstructorUsedError;

  @JsonKey(ignore: true)
  $TestMetadataCopyWith<TestMetadata> get copyWith =>
      throw _privateConstructorUsedError;
}

/// @nodoc
abstract class $TestMetadataCopyWith<$Res> {
  factory $TestMetadataCopyWith(
          TestMetadata value, $Res Function(TestMetadata) then) =
      _$TestMetadataCopyWithImpl<$Res, TestMetadata>;
  @useResult
  $Res call(
      {String problemId,
      String testId,
      String srcTestRootFolder,
      String destTestRootFolder,
      String inputFileName,
      String outputFileName});
}

/// @nodoc
class _$TestMetadataCopyWithImpl<$Res, $Val extends TestMetadata>
    implements $TestMetadataCopyWith<$Res> {
  _$TestMetadataCopyWithImpl(this._value, this._then);

  // ignore: unused_field
  final $Val _value;
  // ignore: unused_field
  final $Res Function($Val) _then;

  @pragma('vm:prefer-inline')
  @override
  $Res call({
    Object? problemId = null,
    Object? testId = null,
    Object? srcTestRootFolder = null,
    Object? destTestRootFolder = null,
    Object? inputFileName = null,
    Object? outputFileName = null,
  }) {
    return _then(_value.copyWith(
      problemId: null == problemId
          ? _value.problemId
          : problemId // ignore: cast_nullable_to_non_nullable
              as String,
      testId: null == testId
          ? _value.testId
          : testId // ignore: cast_nullable_to_non_nullable
              as String,
      srcTestRootFolder: null == srcTestRootFolder
          ? _value.srcTestRootFolder
          : srcTestRootFolder // ignore: cast_nullable_to_non_nullable
              as String,
      destTestRootFolder: null == destTestRootFolder
          ? _value.destTestRootFolder
          : destTestRootFolder // ignore: cast_nullable_to_non_nullable
              as String,
      inputFileName: null == inputFileName
          ? _value.inputFileName
          : inputFileName // ignore: cast_nullable_to_non_nullable
              as String,
      outputFileName: null == outputFileName
          ? _value.outputFileName
          : outputFileName // ignore: cast_nullable_to_non_nullable
              as String,
    ) as $Val);
  }
}

/// @nodoc
abstract class _$$_TestMetadataCopyWith<$Res>
    implements $TestMetadataCopyWith<$Res> {
  factory _$$_TestMetadataCopyWith(
          _$_TestMetadata value, $Res Function(_$_TestMetadata) then) =
      __$$_TestMetadataCopyWithImpl<$Res>;
  @override
  @useResult
  $Res call(
      {String problemId,
      String testId,
      String srcTestRootFolder,
      String destTestRootFolder,
      String inputFileName,
      String outputFileName});
}

/// @nodoc
class __$$_TestMetadataCopyWithImpl<$Res>
    extends _$TestMetadataCopyWithImpl<$Res, _$_TestMetadata>
    implements _$$_TestMetadataCopyWith<$Res> {
  __$$_TestMetadataCopyWithImpl(
      _$_TestMetadata _value, $Res Function(_$_TestMetadata) _then)
      : super(_value, _then);

  @pragma('vm:prefer-inline')
  @override
  $Res call({
    Object? problemId = null,
    Object? testId = null,
    Object? srcTestRootFolder = null,
    Object? destTestRootFolder = null,
    Object? inputFileName = null,
    Object? outputFileName = null,
  }) {
    return _then(_$_TestMetadata(
      problemId: null == problemId
          ? _value.problemId
          : problemId // ignore: cast_nullable_to_non_nullable
              as String,
      testId: null == testId
          ? _value.testId
          : testId // ignore: cast_nullable_to_non_nullable
              as String,
      srcTestRootFolder: null == srcTestRootFolder
          ? _value.srcTestRootFolder
          : srcTestRootFolder // ignore: cast_nullable_to_non_nullable
              as String,
      destTestRootFolder: null == destTestRootFolder
          ? _value.destTestRootFolder
          : destTestRootFolder // ignore: cast_nullable_to_non_nullable
              as String,
      inputFileName: null == inputFileName
          ? _value.inputFileName
          : inputFileName // ignore: cast_nullable_to_non_nullable
              as String,
      outputFileName: null == outputFileName
          ? _value.outputFileName
          : outputFileName // ignore: cast_nullable_to_non_nullable
              as String,
    ));
  }
}

/// @nodoc

class _$_TestMetadata extends _TestMetadata {
  const _$_TestMetadata(
      {required this.problemId,
      required this.testId,
      required this.srcTestRootFolder,
      required this.destTestRootFolder,
      this.inputFileName = "input.txt",
      this.outputFileName = "output.txt"})
      : super._();

  @override
  final String problemId;
  @override
  final String testId;
  @override
  final String srcTestRootFolder;
  @override
  final String destTestRootFolder;
  @override
  @JsonKey()
  final String inputFileName;
  @override
  @JsonKey()
  final String outputFileName;

  @override
  String toString() {
    return 'TestMetadata(problemId: $problemId, testId: $testId, srcTestRootFolder: $srcTestRootFolder, destTestRootFolder: $destTestRootFolder, inputFileName: $inputFileName, outputFileName: $outputFileName)';
  }

  @override
  bool operator ==(dynamic other) {
    return identical(this, other) ||
        (other.runtimeType == runtimeType &&
            other is _$_TestMetadata &&
            (identical(other.problemId, problemId) ||
                other.problemId == problemId) &&
            (identical(other.testId, testId) || other.testId == testId) &&
            (identical(other.srcTestRootFolder, srcTestRootFolder) ||
                other.srcTestRootFolder == srcTestRootFolder) &&
            (identical(other.destTestRootFolder, destTestRootFolder) ||
                other.destTestRootFolder == destTestRootFolder) &&
            (identical(other.inputFileName, inputFileName) ||
                other.inputFileName == inputFileName) &&
            (identical(other.outputFileName, outputFileName) ||
                other.outputFileName == outputFileName));
  }

  @override
  int get hashCode => Object.hash(runtimeType, problemId, testId,
      srcTestRootFolder, destTestRootFolder, inputFileName, outputFileName);

  @JsonKey(ignore: true)
  @override
  @pragma('vm:prefer-inline')
  _$$_TestMetadataCopyWith<_$_TestMetadata> get copyWith =>
      __$$_TestMetadataCopyWithImpl<_$_TestMetadata>(this, _$identity);
}

abstract class _TestMetadata extends TestMetadata {
  const factory _TestMetadata(
      {required final String problemId,
      required final String testId,
      required final String srcTestRootFolder,
      required final String destTestRootFolder,
      final String inputFileName,
      final String outputFileName}) = _$_TestMetadata;
  const _TestMetadata._() : super._();

  @override
  String get problemId;
  @override
  String get testId;
  @override
  String get srcTestRootFolder;
  @override
  String get destTestRootFolder;
  @override
  String get inputFileName;
  @override
  String get outputFileName;
  @override
  @JsonKey(ignore: true)
  _$$_TestMetadataCopyWith<_$_TestMetadata> get copyWith =>
      throw _privateConstructorUsedError;
}
