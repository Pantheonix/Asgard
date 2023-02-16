///
//  Generated code. Do not modify.
//  source: hermes.proto
//
// @dart = 2.12
// ignore_for_file: annotate_overrides,camel_case_types,constant_identifier_names,deprecated_member_use_from_same_package,directives_ordering,library_prefixes,non_constant_identifier_names,prefer_final_fields,return_of_invalid_type,unnecessary_const,unnecessary_import,unnecessary_this,unused_import,unused_shown_name

import 'dart:core' as $core;
import 'dart:convert' as $convert;
import 'dart:typed_data' as $typed_data;
@$core.Deprecated('Use uploadStatusCodeDescriptor instead')
const UploadStatusCode$json = const {
  '1': 'UploadStatusCode',
  '2': const [
    const {'1': 'Ok', '2': 0},
    const {'1': 'Failed', '2': 1},
    const {'1': 'Unknown', '2': 2},
  ],
};

/// Descriptor for `UploadStatusCode`. Decode as a `google.protobuf.EnumDescriptorProto`.
final $typed_data.Uint8List uploadStatusCodeDescriptor = $convert.base64Decode('ChBVcGxvYWRTdGF0dXNDb2RlEgYKAk9rEAASCgoGRmFpbGVkEAESCwoHVW5rbm93bhAC');
@$core.Deprecated('Use uploadRequestDescriptor instead')
const UploadRequest$json = const {
  '1': 'UploadRequest',
  '2': const [
    const {'1': 'metadata', '3': 1, '4': 1, '5': 11, '6': '.asgard.hermes.Metadata', '9': 0, '10': 'metadata'},
    const {'1': 'chunk', '3': 2, '4': 1, '5': 11, '6': '.asgard.hermes.Chunk', '9': 0, '10': 'chunk'},
  ],
  '8': const [
    const {'1': 'packet'},
  ],
};

/// Descriptor for `UploadRequest`. Decode as a `google.protobuf.DescriptorProto`.
final $typed_data.Uint8List uploadRequestDescriptor = $convert.base64Decode('Cg1VcGxvYWRSZXF1ZXN0EjUKCG1ldGFkYXRhGAEgASgLMhcuYXNnYXJkLmhlcm1lcy5NZXRhZGF0YUgAUghtZXRhZGF0YRIsCgVjaHVuaxgCIAEoCzIULmFzZ2FyZC5oZXJtZXMuQ2h1bmtIAFIFY2h1bmtCCAoGcGFja2V0');
@$core.Deprecated('Use uploadResponseDescriptor instead')
const UploadResponse$json = const {
  '1': 'UploadResponse',
  '2': const [
    const {'1': 'message', '3': 1, '4': 1, '5': 9, '10': 'message'},
    const {'1': 'code', '3': 2, '4': 1, '5': 14, '6': '.asgard.hermes.UploadStatusCode', '10': 'code'},
  ],
};

/// Descriptor for `UploadResponse`. Decode as a `google.protobuf.DescriptorProto`.
final $typed_data.Uint8List uploadResponseDescriptor = $convert.base64Decode('Cg5VcGxvYWRSZXNwb25zZRIYCgdtZXNzYWdlGAEgASgJUgdtZXNzYWdlEjMKBGNvZGUYAiABKA4yHy5hc2dhcmQuaGVybWVzLlVwbG9hZFN0YXR1c0NvZGVSBGNvZGU=');
@$core.Deprecated('Use metadataDescriptor instead')
const Metadata$json = const {
  '1': 'Metadata',
  '2': const [
    const {'1': 'problem_id', '3': 1, '4': 1, '5': 9, '10': 'problemId'},
    const {'1': 'test_id', '3': 2, '4': 1, '5': 9, '10': 'testId'},
    const {'1': 'test_size', '3': 3, '4': 1, '5': 5, '10': 'testSize'},
  ],
};

/// Descriptor for `Metadata`. Decode as a `google.protobuf.DescriptorProto`.
final $typed_data.Uint8List metadataDescriptor = $convert.base64Decode('CghNZXRhZGF0YRIdCgpwcm9ibGVtX2lkGAEgASgJUglwcm9ibGVtSWQSFwoHdGVzdF9pZBgCIAEoCVIGdGVzdElkEhsKCXRlc3Rfc2l6ZRgDIAEoBVIIdGVzdFNpemU=');
@$core.Deprecated('Use chunkDescriptor instead')
const Chunk$json = const {
  '1': 'Chunk',
  '2': const [
    const {'1': 'data', '3': 1, '4': 1, '5': 12, '10': 'data'},
  ],
};

/// Descriptor for `Chunk`. Decode as a `google.protobuf.DescriptorProto`.
final $typed_data.Uint8List chunkDescriptor = $convert.base64Decode('CgVDaHVuaxISCgRkYXRhGAEgASgMUgRkYXRh');
