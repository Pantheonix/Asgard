///
//  Generated code. Do not modify.
//  source: hermes.proto
//
// @dart = 2.12
// ignore_for_file: annotate_overrides,camel_case_types,constant_identifier_names,directives_ordering,library_prefixes,non_constant_identifier_names,prefer_final_fields,return_of_invalid_type,unnecessary_const,unnecessary_import,unnecessary_this,unused_import,unused_shown_name

// ignore_for_file: UNDEFINED_SHOWN_NAME
import 'dart:core' as $core;
import 'package:protobuf/protobuf.dart' as $pb;

class UploadStatusCode extends $pb.ProtobufEnum {
  static const UploadStatusCode Ok = UploadStatusCode._(0, const $core.bool.fromEnvironment('protobuf.omit_enum_names') ? '' : 'Ok');
  static const UploadStatusCode Failed = UploadStatusCode._(1, const $core.bool.fromEnvironment('protobuf.omit_enum_names') ? '' : 'Failed');
  static const UploadStatusCode Unknown = UploadStatusCode._(2, const $core.bool.fromEnvironment('protobuf.omit_enum_names') ? '' : 'Unknown');

  static const $core.List<UploadStatusCode> values = <UploadStatusCode> [
    Ok,
    Failed,
    Unknown,
  ];

  static final $core.Map<$core.int, UploadStatusCode> _byValue = $pb.ProtobufEnum.initByValue(values);
  static UploadStatusCode? valueOf($core.int value) => _byValue[value];

  const UploadStatusCode._($core.int v, $core.String n) : super(v, n);
}

