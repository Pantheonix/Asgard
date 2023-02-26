///
//  Generated code. Do not modify.
//  source: hermes.proto
//
// @dart = 2.12
// ignore_for_file: annotate_overrides,camel_case_types,constant_identifier_names,directives_ordering,library_prefixes,non_constant_identifier_names,prefer_final_fields,return_of_invalid_type,unnecessary_const,unnecessary_import,unnecessary_this,unused_import,unused_shown_name

// ignore_for_file: UNDEFINED_SHOWN_NAME
import 'dart:core' as $core;
import 'package:protobuf/protobuf.dart' as $pb;

class StatusCode extends $pb.ProtobufEnum {
  static const StatusCode Ok = StatusCode._(0, const $core.bool.fromEnvironment('protobuf.omit_enum_names') ? '' : 'Ok');
  static const StatusCode Failed = StatusCode._(1, const $core.bool.fromEnvironment('protobuf.omit_enum_names') ? '' : 'Failed');
  static const StatusCode Unknown = StatusCode._(2, const $core.bool.fromEnvironment('protobuf.omit_enum_names') ? '' : 'Unknown');

  static const $core.List<StatusCode> values = <StatusCode> [
    Ok,
    Failed,
    Unknown,
  ];

  static final $core.Map<$core.int, StatusCode> _byValue = $pb.ProtobufEnum.initByValue(values);
  static StatusCode? valueOf($core.int value) => _byValue[value];

  const StatusCode._($core.int v, $core.String n) : super(v, n);
}

