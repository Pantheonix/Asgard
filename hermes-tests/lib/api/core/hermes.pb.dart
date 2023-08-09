///
//  Generated code. Do not modify.
//  source: hermes.proto
//
// @dart = 2.12
// ignore_for_file: annotate_overrides,camel_case_types,constant_identifier_names,directives_ordering,library_prefixes,non_constant_identifier_names,prefer_final_fields,return_of_invalid_type,unnecessary_const,unnecessary_import,unnecessary_this,unused_import,unused_shown_name

import 'dart:core' as $core;

import 'package:protobuf/protobuf.dart' as $pb;

import 'hermes.pbenum.dart';

export 'hermes.pbenum.dart';

enum UploadRequest_Packet {
  metadata, 
  chunk, 
  notSet
}

class UploadRequest extends $pb.GeneratedMessage {
  static const $core.Map<$core.int, UploadRequest_Packet> _UploadRequest_PacketByTag = {
    1 : UploadRequest_Packet.metadata,
    2 : UploadRequest_Packet.chunk,
    0 : UploadRequest_Packet.notSet
  };
  static final $pb.BuilderInfo _i = $pb.BuilderInfo(const $core.bool.fromEnvironment('protobuf.omit_message_names') ? '' : 'UploadRequest', package: const $pb.PackageName(const $core.bool.fromEnvironment('protobuf.omit_message_names') ? '' : 'asgard.hermes'), createEmptyInstance: create)
    ..oo(0, [1, 2])
    ..aOM<Metadata>(1, const $core.bool.fromEnvironment('protobuf.omit_field_names') ? '' : 'metadata', subBuilder: Metadata.create)
    ..aOM<Chunk>(2, const $core.bool.fromEnvironment('protobuf.omit_field_names') ? '' : 'chunk', subBuilder: Chunk.create)
    ..hasRequiredFields = false
  ;

  UploadRequest._() : super();
  factory UploadRequest({
    Metadata? metadata,
    Chunk? chunk,
  }) {
    final _result = create();
    if (metadata != null) {
      _result.metadata = metadata;
    }
    if (chunk != null) {
      _result.chunk = chunk;
    }
    return _result;
  }
  factory UploadRequest.fromBuffer($core.List<$core.int> i, [$pb.ExtensionRegistry r = $pb.ExtensionRegistry.EMPTY]) => create()..mergeFromBuffer(i, r);
  factory UploadRequest.fromJson($core.String i, [$pb.ExtensionRegistry r = $pb.ExtensionRegistry.EMPTY]) => create()..mergeFromJson(i, r);
  @$core.Deprecated(
  'Using this can add significant overhead to your binary. '
  'Use [GeneratedMessageGenericExtensions.deepCopy] instead. '
  'Will be removed in next major version')
  UploadRequest clone() => UploadRequest()..mergeFromMessage(this);
  @$core.Deprecated(
  'Using this can add significant overhead to your binary. '
  'Use [GeneratedMessageGenericExtensions.rebuild] instead. '
  'Will be removed in next major version')
  UploadRequest copyWith(void Function(UploadRequest) updates) => super.copyWith((message) => updates(message as UploadRequest)) as UploadRequest; // ignore: deprecated_member_use
  $pb.BuilderInfo get info_ => _i;
  @$core.pragma('dart2js:noInline')
  static UploadRequest create() => UploadRequest._();
  UploadRequest createEmptyInstance() => create();
  static $pb.PbList<UploadRequest> createRepeated() => $pb.PbList<UploadRequest>();
  @$core.pragma('dart2js:noInline')
  static UploadRequest getDefault() => _defaultInstance ??= $pb.GeneratedMessage.$_defaultFor<UploadRequest>(create);
  static UploadRequest? _defaultInstance;

  UploadRequest_Packet whichPacket() => _UploadRequest_PacketByTag[$_whichOneof(0)]!;
  void clearPacket() => clearField($_whichOneof(0));

  @$pb.TagNumber(1)
  Metadata get metadata => $_getN(0);
  @$pb.TagNumber(1)
  set metadata(Metadata v) { setField(1, v); }
  @$pb.TagNumber(1)
  $core.bool hasMetadata() => $_has(0);
  @$pb.TagNumber(1)
  void clearMetadata() => clearField(1);
  @$pb.TagNumber(1)
  Metadata ensureMetadata() => $_ensure(0);

  @$pb.TagNumber(2)
  Chunk get chunk => $_getN(1);
  @$pb.TagNumber(2)
  set chunk(Chunk v) { setField(2, v); }
  @$pb.TagNumber(2)
  $core.bool hasChunk() => $_has(1);
  @$pb.TagNumber(2)
  void clearChunk() => clearField(2);
  @$pb.TagNumber(2)
  Chunk ensureChunk() => $_ensure(1);
}

class UploadResponse extends $pb.GeneratedMessage {
  static final $pb.BuilderInfo _i = $pb.BuilderInfo(const $core.bool.fromEnvironment('protobuf.omit_message_names') ? '' : 'UploadResponse', package: const $pb.PackageName(const $core.bool.fromEnvironment('protobuf.omit_message_names') ? '' : 'asgard.hermes'), createEmptyInstance: create)
    ..aOM<StatusResponse>(1, const $core.bool.fromEnvironment('protobuf.omit_field_names') ? '' : 'status', subBuilder: StatusResponse.create)
    ..hasRequiredFields = false
  ;

  UploadResponse._() : super();
  factory UploadResponse({
    StatusResponse? status,
  }) {
    final _result = create();
    if (status != null) {
      _result.status = status;
    }
    return _result;
  }
  factory UploadResponse.fromBuffer($core.List<$core.int> i, [$pb.ExtensionRegistry r = $pb.ExtensionRegistry.EMPTY]) => create()..mergeFromBuffer(i, r);
  factory UploadResponse.fromJson($core.String i, [$pb.ExtensionRegistry r = $pb.ExtensionRegistry.EMPTY]) => create()..mergeFromJson(i, r);
  @$core.Deprecated(
  'Using this can add significant overhead to your binary. '
  'Use [GeneratedMessageGenericExtensions.deepCopy] instead. '
  'Will be removed in next major version')
  UploadResponse clone() => UploadResponse()..mergeFromMessage(this);
  @$core.Deprecated(
  'Using this can add significant overhead to your binary. '
  'Use [GeneratedMessageGenericExtensions.rebuild] instead. '
  'Will be removed in next major version')
  UploadResponse copyWith(void Function(UploadResponse) updates) => super.copyWith((message) => updates(message as UploadResponse)) as UploadResponse; // ignore: deprecated_member_use
  $pb.BuilderInfo get info_ => _i;
  @$core.pragma('dart2js:noInline')
  static UploadResponse create() => UploadResponse._();
  UploadResponse createEmptyInstance() => create();
  static $pb.PbList<UploadResponse> createRepeated() => $pb.PbList<UploadResponse>();
  @$core.pragma('dart2js:noInline')
  static UploadResponse getDefault() => _defaultInstance ??= $pb.GeneratedMessage.$_defaultFor<UploadResponse>(create);
  static UploadResponse? _defaultInstance;

  @$pb.TagNumber(1)
  StatusResponse get status => $_getN(0);
  @$pb.TagNumber(1)
  set status(StatusResponse v) { setField(1, v); }
  @$pb.TagNumber(1)
  $core.bool hasStatus() => $_has(0);
  @$pb.TagNumber(1)
  void clearStatus() => clearField(1);
  @$pb.TagNumber(1)
  StatusResponse ensureStatus() => $_ensure(0);
}

class DownloadRequest extends $pb.GeneratedMessage {
  static final $pb.BuilderInfo _i = $pb.BuilderInfo(const $core.bool.fromEnvironment('protobuf.omit_message_names') ? '' : 'DownloadRequest', package: const $pb.PackageName(const $core.bool.fromEnvironment('protobuf.omit_message_names') ? '' : 'asgard.hermes'), createEmptyInstance: create)
    ..aOS(1, const $core.bool.fromEnvironment('protobuf.omit_field_names') ? '' : 'problemId')
    ..aOS(2, const $core.bool.fromEnvironment('protobuf.omit_field_names') ? '' : 'testId')
    ..hasRequiredFields = false
  ;

  DownloadRequest._() : super();
  factory DownloadRequest({
    $core.String? problemId,
    $core.String? testId,
  }) {
    final _result = create();
    if (problemId != null) {
      _result.problemId = problemId;
    }
    if (testId != null) {
      _result.testId = testId;
    }
    return _result;
  }
  factory DownloadRequest.fromBuffer($core.List<$core.int> i, [$pb.ExtensionRegistry r = $pb.ExtensionRegistry.EMPTY]) => create()..mergeFromBuffer(i, r);
  factory DownloadRequest.fromJson($core.String i, [$pb.ExtensionRegistry r = $pb.ExtensionRegistry.EMPTY]) => create()..mergeFromJson(i, r);
  @$core.Deprecated(
  'Using this can add significant overhead to your binary. '
  'Use [GeneratedMessageGenericExtensions.deepCopy] instead. '
  'Will be removed in next major version')
  DownloadRequest clone() => DownloadRequest()..mergeFromMessage(this);
  @$core.Deprecated(
  'Using this can add significant overhead to your binary. '
  'Use [GeneratedMessageGenericExtensions.rebuild] instead. '
  'Will be removed in next major version')
  DownloadRequest copyWith(void Function(DownloadRequest) updates) => super.copyWith((message) => updates(message as DownloadRequest)) as DownloadRequest; // ignore: deprecated_member_use
  $pb.BuilderInfo get info_ => _i;
  @$core.pragma('dart2js:noInline')
  static DownloadRequest create() => DownloadRequest._();
  DownloadRequest createEmptyInstance() => create();
  static $pb.PbList<DownloadRequest> createRepeated() => $pb.PbList<DownloadRequest>();
  @$core.pragma('dart2js:noInline')
  static DownloadRequest getDefault() => _defaultInstance ??= $pb.GeneratedMessage.$_defaultFor<DownloadRequest>(create);
  static DownloadRequest? _defaultInstance;

  @$pb.TagNumber(1)
  $core.String get problemId => $_getSZ(0);
  @$pb.TagNumber(1)
  set problemId($core.String v) { $_setString(0, v); }
  @$pb.TagNumber(1)
  $core.bool hasProblemId() => $_has(0);
  @$pb.TagNumber(1)
  void clearProblemId() => clearField(1);

  @$pb.TagNumber(2)
  $core.String get testId => $_getSZ(1);
  @$pb.TagNumber(2)
  set testId($core.String v) { $_setString(1, v); }
  @$pb.TagNumber(2)
  $core.bool hasTestId() => $_has(1);
  @$pb.TagNumber(2)
  void clearTestId() => clearField(2);
}

enum DownloadResponse_Packet {
  metadata, 
  chunk, 
  status, 
  notSet
}

class DownloadResponse extends $pb.GeneratedMessage {
  static const $core.Map<$core.int, DownloadResponse_Packet> _DownloadResponse_PacketByTag = {
    1 : DownloadResponse_Packet.metadata,
    2 : DownloadResponse_Packet.chunk,
    3 : DownloadResponse_Packet.status,
    0 : DownloadResponse_Packet.notSet
  };
  static final $pb.BuilderInfo _i = $pb.BuilderInfo(const $core.bool.fromEnvironment('protobuf.omit_message_names') ? '' : 'DownloadResponse', package: const $pb.PackageName(const $core.bool.fromEnvironment('protobuf.omit_message_names') ? '' : 'asgard.hermes'), createEmptyInstance: create)
    ..oo(0, [1, 2, 3])
    ..aOM<Metadata>(1, const $core.bool.fromEnvironment('protobuf.omit_field_names') ? '' : 'metadata', subBuilder: Metadata.create)
    ..aOM<Chunk>(2, const $core.bool.fromEnvironment('protobuf.omit_field_names') ? '' : 'chunk', subBuilder: Chunk.create)
    ..aOM<StatusResponse>(3, const $core.bool.fromEnvironment('protobuf.omit_field_names') ? '' : 'status', subBuilder: StatusResponse.create)
    ..hasRequiredFields = false
  ;

  DownloadResponse._() : super();
  factory DownloadResponse({
    Metadata? metadata,
    Chunk? chunk,
    StatusResponse? status,
  }) {
    final _result = create();
    if (metadata != null) {
      _result.metadata = metadata;
    }
    if (chunk != null) {
      _result.chunk = chunk;
    }
    if (status != null) {
      _result.status = status;
    }
    return _result;
  }
  factory DownloadResponse.fromBuffer($core.List<$core.int> i, [$pb.ExtensionRegistry r = $pb.ExtensionRegistry.EMPTY]) => create()..mergeFromBuffer(i, r);
  factory DownloadResponse.fromJson($core.String i, [$pb.ExtensionRegistry r = $pb.ExtensionRegistry.EMPTY]) => create()..mergeFromJson(i, r);
  @$core.Deprecated(
  'Using this can add significant overhead to your binary. '
  'Use [GeneratedMessageGenericExtensions.deepCopy] instead. '
  'Will be removed in next major version')
  DownloadResponse clone() => DownloadResponse()..mergeFromMessage(this);
  @$core.Deprecated(
  'Using this can add significant overhead to your binary. '
  'Use [GeneratedMessageGenericExtensions.rebuild] instead. '
  'Will be removed in next major version')
  DownloadResponse copyWith(void Function(DownloadResponse) updates) => super.copyWith((message) => updates(message as DownloadResponse)) as DownloadResponse; // ignore: deprecated_member_use
  $pb.BuilderInfo get info_ => _i;
  @$core.pragma('dart2js:noInline')
  static DownloadResponse create() => DownloadResponse._();
  DownloadResponse createEmptyInstance() => create();
  static $pb.PbList<DownloadResponse> createRepeated() => $pb.PbList<DownloadResponse>();
  @$core.pragma('dart2js:noInline')
  static DownloadResponse getDefault() => _defaultInstance ??= $pb.GeneratedMessage.$_defaultFor<DownloadResponse>(create);
  static DownloadResponse? _defaultInstance;

  DownloadResponse_Packet whichPacket() => _DownloadResponse_PacketByTag[$_whichOneof(0)]!;
  void clearPacket() => clearField($_whichOneof(0));

  @$pb.TagNumber(1)
  Metadata get metadata => $_getN(0);
  @$pb.TagNumber(1)
  set metadata(Metadata v) { setField(1, v); }
  @$pb.TagNumber(1)
  $core.bool hasMetadata() => $_has(0);
  @$pb.TagNumber(1)
  void clearMetadata() => clearField(1);
  @$pb.TagNumber(1)
  Metadata ensureMetadata() => $_ensure(0);

  @$pb.TagNumber(2)
  Chunk get chunk => $_getN(1);
  @$pb.TagNumber(2)
  set chunk(Chunk v) { setField(2, v); }
  @$pb.TagNumber(2)
  $core.bool hasChunk() => $_has(1);
  @$pb.TagNumber(2)
  void clearChunk() => clearField(2);
  @$pb.TagNumber(2)
  Chunk ensureChunk() => $_ensure(1);

  @$pb.TagNumber(3)
  StatusResponse get status => $_getN(2);
  @$pb.TagNumber(3)
  set status(StatusResponse v) { setField(3, v); }
  @$pb.TagNumber(3)
  $core.bool hasStatus() => $_has(2);
  @$pb.TagNumber(3)
  void clearStatus() => clearField(3);
  @$pb.TagNumber(3)
  StatusResponse ensureStatus() => $_ensure(2);
}

class GetDownloadLinkForTestRequest extends $pb.GeneratedMessage {
  static final $pb.BuilderInfo _i = $pb.BuilderInfo(const $core.bool.fromEnvironment('protobuf.omit_message_names') ? '' : 'GetDownloadLinkForTestRequest', package: const $pb.PackageName(const $core.bool.fromEnvironment('protobuf.omit_message_names') ? '' : 'asgard.hermes'), createEmptyInstance: create)
    ..aOS(1, const $core.bool.fromEnvironment('protobuf.omit_field_names') ? '' : 'problemId')
    ..aOS(2, const $core.bool.fromEnvironment('protobuf.omit_field_names') ? '' : 'testId')
    ..hasRequiredFields = false
  ;

  GetDownloadLinkForTestRequest._() : super();
  factory GetDownloadLinkForTestRequest({
    $core.String? problemId,
    $core.String? testId,
  }) {
    final _result = create();
    if (problemId != null) {
      _result.problemId = problemId;
    }
    if (testId != null) {
      _result.testId = testId;
    }
    return _result;
  }
  factory GetDownloadLinkForTestRequest.fromBuffer($core.List<$core.int> i, [$pb.ExtensionRegistry r = $pb.ExtensionRegistry.EMPTY]) => create()..mergeFromBuffer(i, r);
  factory GetDownloadLinkForTestRequest.fromJson($core.String i, [$pb.ExtensionRegistry r = $pb.ExtensionRegistry.EMPTY]) => create()..mergeFromJson(i, r);
  @$core.Deprecated(
  'Using this can add significant overhead to your binary. '
  'Use [GeneratedMessageGenericExtensions.deepCopy] instead. '
  'Will be removed in next major version')
  GetDownloadLinkForTestRequest clone() => GetDownloadLinkForTestRequest()..mergeFromMessage(this);
  @$core.Deprecated(
  'Using this can add significant overhead to your binary. '
  'Use [GeneratedMessageGenericExtensions.rebuild] instead. '
  'Will be removed in next major version')
  GetDownloadLinkForTestRequest copyWith(void Function(GetDownloadLinkForTestRequest) updates) => super.copyWith((message) => updates(message as GetDownloadLinkForTestRequest)) as GetDownloadLinkForTestRequest; // ignore: deprecated_member_use
  $pb.BuilderInfo get info_ => _i;
  @$core.pragma('dart2js:noInline')
  static GetDownloadLinkForTestRequest create() => GetDownloadLinkForTestRequest._();
  GetDownloadLinkForTestRequest createEmptyInstance() => create();
  static $pb.PbList<GetDownloadLinkForTestRequest> createRepeated() => $pb.PbList<GetDownloadLinkForTestRequest>();
  @$core.pragma('dart2js:noInline')
  static GetDownloadLinkForTestRequest getDefault() => _defaultInstance ??= $pb.GeneratedMessage.$_defaultFor<GetDownloadLinkForTestRequest>(create);
  static GetDownloadLinkForTestRequest? _defaultInstance;

  @$pb.TagNumber(1)
  $core.String get problemId => $_getSZ(0);
  @$pb.TagNumber(1)
  set problemId($core.String v) { $_setString(0, v); }
  @$pb.TagNumber(1)
  $core.bool hasProblemId() => $_has(0);
  @$pb.TagNumber(1)
  void clearProblemId() => clearField(1);

  @$pb.TagNumber(2)
  $core.String get testId => $_getSZ(1);
  @$pb.TagNumber(2)
  set testId($core.String v) { $_setString(1, v); }
  @$pb.TagNumber(2)
  $core.bool hasTestId() => $_has(1);
  @$pb.TagNumber(2)
  void clearTestId() => clearField(2);
}

class GetDownloadLinkForTestResponse extends $pb.GeneratedMessage {
  static final $pb.BuilderInfo _i = $pb.BuilderInfo(const $core.bool.fromEnvironment('protobuf.omit_message_names') ? '' : 'GetDownloadLinkForTestResponse', package: const $pb.PackageName(const $core.bool.fromEnvironment('protobuf.omit_message_names') ? '' : 'asgard.hermes'), createEmptyInstance: create)
    ..aOS(1, const $core.bool.fromEnvironment('protobuf.omit_field_names') ? '' : 'inputLink')
    ..aOS(2, const $core.bool.fromEnvironment('protobuf.omit_field_names') ? '' : 'outputLink')
    ..aOM<StatusResponse>(3, const $core.bool.fromEnvironment('protobuf.omit_field_names') ? '' : 'status', subBuilder: StatusResponse.create)
    ..hasRequiredFields = false
  ;

  GetDownloadLinkForTestResponse._() : super();
  factory GetDownloadLinkForTestResponse({
    $core.String? inputLink,
    $core.String? outputLink,
    StatusResponse? status,
  }) {
    final _result = create();
    if (inputLink != null) {
      _result.inputLink = inputLink;
    }
    if (outputLink != null) {
      _result.outputLink = outputLink;
    }
    if (status != null) {
      _result.status = status;
    }
    return _result;
  }
  factory GetDownloadLinkForTestResponse.fromBuffer($core.List<$core.int> i, [$pb.ExtensionRegistry r = $pb.ExtensionRegistry.EMPTY]) => create()..mergeFromBuffer(i, r);
  factory GetDownloadLinkForTestResponse.fromJson($core.String i, [$pb.ExtensionRegistry r = $pb.ExtensionRegistry.EMPTY]) => create()..mergeFromJson(i, r);
  @$core.Deprecated(
  'Using this can add significant overhead to your binary. '
  'Use [GeneratedMessageGenericExtensions.deepCopy] instead. '
  'Will be removed in next major version')
  GetDownloadLinkForTestResponse clone() => GetDownloadLinkForTestResponse()..mergeFromMessage(this);
  @$core.Deprecated(
  'Using this can add significant overhead to your binary. '
  'Use [GeneratedMessageGenericExtensions.rebuild] instead. '
  'Will be removed in next major version')
  GetDownloadLinkForTestResponse copyWith(void Function(GetDownloadLinkForTestResponse) updates) => super.copyWith((message) => updates(message as GetDownloadLinkForTestResponse)) as GetDownloadLinkForTestResponse; // ignore: deprecated_member_use
  $pb.BuilderInfo get info_ => _i;
  @$core.pragma('dart2js:noInline')
  static GetDownloadLinkForTestResponse create() => GetDownloadLinkForTestResponse._();
  GetDownloadLinkForTestResponse createEmptyInstance() => create();
  static $pb.PbList<GetDownloadLinkForTestResponse> createRepeated() => $pb.PbList<GetDownloadLinkForTestResponse>();
  @$core.pragma('dart2js:noInline')
  static GetDownloadLinkForTestResponse getDefault() => _defaultInstance ??= $pb.GeneratedMessage.$_defaultFor<GetDownloadLinkForTestResponse>(create);
  static GetDownloadLinkForTestResponse? _defaultInstance;

  @$pb.TagNumber(1)
  $core.String get inputLink => $_getSZ(0);
  @$pb.TagNumber(1)
  set inputLink($core.String v) { $_setString(0, v); }
  @$pb.TagNumber(1)
  $core.bool hasInputLink() => $_has(0);
  @$pb.TagNumber(1)
  void clearInputLink() => clearField(1);

  @$pb.TagNumber(2)
  $core.String get outputLink => $_getSZ(1);
  @$pb.TagNumber(2)
  set outputLink($core.String v) { $_setString(1, v); }
  @$pb.TagNumber(2)
  $core.bool hasOutputLink() => $_has(1);
  @$pb.TagNumber(2)
  void clearOutputLink() => clearField(2);

  @$pb.TagNumber(3)
  StatusResponse get status => $_getN(2);
  @$pb.TagNumber(3)
  set status(StatusResponse v) { setField(3, v); }
  @$pb.TagNumber(3)
  $core.bool hasStatus() => $_has(2);
  @$pb.TagNumber(3)
  void clearStatus() => clearField(3);
  @$pb.TagNumber(3)
  StatusResponse ensureStatus() => $_ensure(2);
}

class DeleteTestRequest extends $pb.GeneratedMessage {
  static final $pb.BuilderInfo _i = $pb.BuilderInfo(const $core.bool.fromEnvironment('protobuf.omit_message_names') ? '' : 'DeleteTestRequest', package: const $pb.PackageName(const $core.bool.fromEnvironment('protobuf.omit_message_names') ? '' : 'asgard.hermes'), createEmptyInstance: create)
    ..aOS(1, const $core.bool.fromEnvironment('protobuf.omit_field_names') ? '' : 'problemId')
    ..aOS(2, const $core.bool.fromEnvironment('protobuf.omit_field_names') ? '' : 'testId')
    ..hasRequiredFields = false
  ;

  DeleteTestRequest._() : super();
  factory DeleteTestRequest({
    $core.String? problemId,
    $core.String? testId,
  }) {
    final _result = create();
    if (problemId != null) {
      _result.problemId = problemId;
    }
    if (testId != null) {
      _result.testId = testId;
    }
    return _result;
  }
  factory DeleteTestRequest.fromBuffer($core.List<$core.int> i, [$pb.ExtensionRegistry r = $pb.ExtensionRegistry.EMPTY]) => create()..mergeFromBuffer(i, r);
  factory DeleteTestRequest.fromJson($core.String i, [$pb.ExtensionRegistry r = $pb.ExtensionRegistry.EMPTY]) => create()..mergeFromJson(i, r);
  @$core.Deprecated(
  'Using this can add significant overhead to your binary. '
  'Use [GeneratedMessageGenericExtensions.deepCopy] instead. '
  'Will be removed in next major version')
  DeleteTestRequest clone() => DeleteTestRequest()..mergeFromMessage(this);
  @$core.Deprecated(
  'Using this can add significant overhead to your binary. '
  'Use [GeneratedMessageGenericExtensions.rebuild] instead. '
  'Will be removed in next major version')
  DeleteTestRequest copyWith(void Function(DeleteTestRequest) updates) => super.copyWith((message) => updates(message as DeleteTestRequest)) as DeleteTestRequest; // ignore: deprecated_member_use
  $pb.BuilderInfo get info_ => _i;
  @$core.pragma('dart2js:noInline')
  static DeleteTestRequest create() => DeleteTestRequest._();
  DeleteTestRequest createEmptyInstance() => create();
  static $pb.PbList<DeleteTestRequest> createRepeated() => $pb.PbList<DeleteTestRequest>();
  @$core.pragma('dart2js:noInline')
  static DeleteTestRequest getDefault() => _defaultInstance ??= $pb.GeneratedMessage.$_defaultFor<DeleteTestRequest>(create);
  static DeleteTestRequest? _defaultInstance;

  @$pb.TagNumber(1)
  $core.String get problemId => $_getSZ(0);
  @$pb.TagNumber(1)
  set problemId($core.String v) { $_setString(0, v); }
  @$pb.TagNumber(1)
  $core.bool hasProblemId() => $_has(0);
  @$pb.TagNumber(1)
  void clearProblemId() => clearField(1);

  @$pb.TagNumber(2)
  $core.String get testId => $_getSZ(1);
  @$pb.TagNumber(2)
  set testId($core.String v) { $_setString(1, v); }
  @$pb.TagNumber(2)
  $core.bool hasTestId() => $_has(1);
  @$pb.TagNumber(2)
  void clearTestId() => clearField(2);
}

class DeleteTestResponse extends $pb.GeneratedMessage {
  static final $pb.BuilderInfo _i = $pb.BuilderInfo(const $core.bool.fromEnvironment('protobuf.omit_message_names') ? '' : 'DeleteTestResponse', package: const $pb.PackageName(const $core.bool.fromEnvironment('protobuf.omit_message_names') ? '' : 'asgard.hermes'), createEmptyInstance: create)
    ..aOM<StatusResponse>(1, const $core.bool.fromEnvironment('protobuf.omit_field_names') ? '' : 'status', subBuilder: StatusResponse.create)
    ..hasRequiredFields = false
  ;

  DeleteTestResponse._() : super();
  factory DeleteTestResponse({
    StatusResponse? status,
  }) {
    final _result = create();
    if (status != null) {
      _result.status = status;
    }
    return _result;
  }
  factory DeleteTestResponse.fromBuffer($core.List<$core.int> i, [$pb.ExtensionRegistry r = $pb.ExtensionRegistry.EMPTY]) => create()..mergeFromBuffer(i, r);
  factory DeleteTestResponse.fromJson($core.String i, [$pb.ExtensionRegistry r = $pb.ExtensionRegistry.EMPTY]) => create()..mergeFromJson(i, r);
  @$core.Deprecated(
  'Using this can add significant overhead to your binary. '
  'Use [GeneratedMessageGenericExtensions.deepCopy] instead. '
  'Will be removed in next major version')
  DeleteTestResponse clone() => DeleteTestResponse()..mergeFromMessage(this);
  @$core.Deprecated(
  'Using this can add significant overhead to your binary. '
  'Use [GeneratedMessageGenericExtensions.rebuild] instead. '
  'Will be removed in next major version')
  DeleteTestResponse copyWith(void Function(DeleteTestResponse) updates) => super.copyWith((message) => updates(message as DeleteTestResponse)) as DeleteTestResponse; // ignore: deprecated_member_use
  $pb.BuilderInfo get info_ => _i;
  @$core.pragma('dart2js:noInline')
  static DeleteTestResponse create() => DeleteTestResponse._();
  DeleteTestResponse createEmptyInstance() => create();
  static $pb.PbList<DeleteTestResponse> createRepeated() => $pb.PbList<DeleteTestResponse>();
  @$core.pragma('dart2js:noInline')
  static DeleteTestResponse getDefault() => _defaultInstance ??= $pb.GeneratedMessage.$_defaultFor<DeleteTestResponse>(create);
  static DeleteTestResponse? _defaultInstance;

  @$pb.TagNumber(1)
  StatusResponse get status => $_getN(0);
  @$pb.TagNumber(1)
  set status(StatusResponse v) { setField(1, v); }
  @$pb.TagNumber(1)
  $core.bool hasStatus() => $_has(0);
  @$pb.TagNumber(1)
  void clearStatus() => clearField(1);
  @$pb.TagNumber(1)
  StatusResponse ensureStatus() => $_ensure(0);
}

class Metadata extends $pb.GeneratedMessage {
  static final $pb.BuilderInfo _i = $pb.BuilderInfo(const $core.bool.fromEnvironment('protobuf.omit_message_names') ? '' : 'Metadata', package: const $pb.PackageName(const $core.bool.fromEnvironment('protobuf.omit_message_names') ? '' : 'asgard.hermes'), createEmptyInstance: create)
    ..aOS(1, const $core.bool.fromEnvironment('protobuf.omit_field_names') ? '' : 'problemId')
    ..aOS(2, const $core.bool.fromEnvironment('protobuf.omit_field_names') ? '' : 'testId')
    ..a<$core.int>(3, const $core.bool.fromEnvironment('protobuf.omit_field_names') ? '' : 'testSize', $pb.PbFieldType.O3)
    ..hasRequiredFields = false
  ;

  Metadata._() : super();
  factory Metadata({
    $core.String? problemId,
    $core.String? testId,
    $core.int? testSize,
  }) {
    final _result = create();
    if (problemId != null) {
      _result.problemId = problemId;
    }
    if (testId != null) {
      _result.testId = testId;
    }
    if (testSize != null) {
      _result.testSize = testSize;
    }
    return _result;
  }
  factory Metadata.fromBuffer($core.List<$core.int> i, [$pb.ExtensionRegistry r = $pb.ExtensionRegistry.EMPTY]) => create()..mergeFromBuffer(i, r);
  factory Metadata.fromJson($core.String i, [$pb.ExtensionRegistry r = $pb.ExtensionRegistry.EMPTY]) => create()..mergeFromJson(i, r);
  @$core.Deprecated(
  'Using this can add significant overhead to your binary. '
  'Use [GeneratedMessageGenericExtensions.deepCopy] instead. '
  'Will be removed in next major version')
  Metadata clone() => Metadata()..mergeFromMessage(this);
  @$core.Deprecated(
  'Using this can add significant overhead to your binary. '
  'Use [GeneratedMessageGenericExtensions.rebuild] instead. '
  'Will be removed in next major version')
  Metadata copyWith(void Function(Metadata) updates) => super.copyWith((message) => updates(message as Metadata)) as Metadata; // ignore: deprecated_member_use
  $pb.BuilderInfo get info_ => _i;
  @$core.pragma('dart2js:noInline')
  static Metadata create() => Metadata._();
  Metadata createEmptyInstance() => create();
  static $pb.PbList<Metadata> createRepeated() => $pb.PbList<Metadata>();
  @$core.pragma('dart2js:noInline')
  static Metadata getDefault() => _defaultInstance ??= $pb.GeneratedMessage.$_defaultFor<Metadata>(create);
  static Metadata? _defaultInstance;

  @$pb.TagNumber(1)
  $core.String get problemId => $_getSZ(0);
  @$pb.TagNumber(1)
  set problemId($core.String v) { $_setString(0, v); }
  @$pb.TagNumber(1)
  $core.bool hasProblemId() => $_has(0);
  @$pb.TagNumber(1)
  void clearProblemId() => clearField(1);

  @$pb.TagNumber(2)
  $core.String get testId => $_getSZ(1);
  @$pb.TagNumber(2)
  set testId($core.String v) { $_setString(1, v); }
  @$pb.TagNumber(2)
  $core.bool hasTestId() => $_has(1);
  @$pb.TagNumber(2)
  void clearTestId() => clearField(2);

  @$pb.TagNumber(3)
  $core.int get testSize => $_getIZ(2);
  @$pb.TagNumber(3)
  set testSize($core.int v) { $_setSignedInt32(2, v); }
  @$pb.TagNumber(3)
  $core.bool hasTestSize() => $_has(2);
  @$pb.TagNumber(3)
  void clearTestSize() => clearField(3);
}

class Chunk extends $pb.GeneratedMessage {
  static final $pb.BuilderInfo _i = $pb.BuilderInfo(const $core.bool.fromEnvironment('protobuf.omit_message_names') ? '' : 'Chunk', package: const $pb.PackageName(const $core.bool.fromEnvironment('protobuf.omit_message_names') ? '' : 'asgard.hermes'), createEmptyInstance: create)
    ..a<$core.List<$core.int>>(1, const $core.bool.fromEnvironment('protobuf.omit_field_names') ? '' : 'data', $pb.PbFieldType.OY)
    ..hasRequiredFields = false
  ;

  Chunk._() : super();
  factory Chunk({
    $core.List<$core.int>? data,
  }) {
    final _result = create();
    if (data != null) {
      _result.data = data;
    }
    return _result;
  }
  factory Chunk.fromBuffer($core.List<$core.int> i, [$pb.ExtensionRegistry r = $pb.ExtensionRegistry.EMPTY]) => create()..mergeFromBuffer(i, r);
  factory Chunk.fromJson($core.String i, [$pb.ExtensionRegistry r = $pb.ExtensionRegistry.EMPTY]) => create()..mergeFromJson(i, r);
  @$core.Deprecated(
  'Using this can add significant overhead to your binary. '
  'Use [GeneratedMessageGenericExtensions.deepCopy] instead. '
  'Will be removed in next major version')
  Chunk clone() => Chunk()..mergeFromMessage(this);
  @$core.Deprecated(
  'Using this can add significant overhead to your binary. '
  'Use [GeneratedMessageGenericExtensions.rebuild] instead. '
  'Will be removed in next major version')
  Chunk copyWith(void Function(Chunk) updates) => super.copyWith((message) => updates(message as Chunk)) as Chunk; // ignore: deprecated_member_use
  $pb.BuilderInfo get info_ => _i;
  @$core.pragma('dart2js:noInline')
  static Chunk create() => Chunk._();
  Chunk createEmptyInstance() => create();
  static $pb.PbList<Chunk> createRepeated() => $pb.PbList<Chunk>();
  @$core.pragma('dart2js:noInline')
  static Chunk getDefault() => _defaultInstance ??= $pb.GeneratedMessage.$_defaultFor<Chunk>(create);
  static Chunk? _defaultInstance;

  @$pb.TagNumber(1)
  $core.List<$core.int> get data => $_getN(0);
  @$pb.TagNumber(1)
  set data($core.List<$core.int> v) { $_setBytes(0, v); }
  @$pb.TagNumber(1)
  $core.bool hasData() => $_has(0);
  @$pb.TagNumber(1)
  void clearData() => clearField(1);
}

class StatusResponse extends $pb.GeneratedMessage {
  static final $pb.BuilderInfo _i = $pb.BuilderInfo(const $core.bool.fromEnvironment('protobuf.omit_message_names') ? '' : 'StatusResponse', package: const $pb.PackageName(const $core.bool.fromEnvironment('protobuf.omit_message_names') ? '' : 'asgard.hermes'), createEmptyInstance: create)
    ..aOS(1, const $core.bool.fromEnvironment('protobuf.omit_field_names') ? '' : 'message')
    ..e<StatusCode>(2, const $core.bool.fromEnvironment('protobuf.omit_field_names') ? '' : 'code', $pb.PbFieldType.OE, defaultOrMaker: StatusCode.Ok, valueOf: StatusCode.valueOf, enumValues: StatusCode.values)
    ..hasRequiredFields = false
  ;

  StatusResponse._() : super();
  factory StatusResponse({
    $core.String? message,
    StatusCode? code,
  }) {
    final _result = create();
    if (message != null) {
      _result.message = message;
    }
    if (code != null) {
      _result.code = code;
    }
    return _result;
  }
  factory StatusResponse.fromBuffer($core.List<$core.int> i, [$pb.ExtensionRegistry r = $pb.ExtensionRegistry.EMPTY]) => create()..mergeFromBuffer(i, r);
  factory StatusResponse.fromJson($core.String i, [$pb.ExtensionRegistry r = $pb.ExtensionRegistry.EMPTY]) => create()..mergeFromJson(i, r);
  @$core.Deprecated(
  'Using this can add significant overhead to your binary. '
  'Use [GeneratedMessageGenericExtensions.deepCopy] instead. '
  'Will be removed in next major version')
  StatusResponse clone() => StatusResponse()..mergeFromMessage(this);
  @$core.Deprecated(
  'Using this can add significant overhead to your binary. '
  'Use [GeneratedMessageGenericExtensions.rebuild] instead. '
  'Will be removed in next major version')
  StatusResponse copyWith(void Function(StatusResponse) updates) => super.copyWith((message) => updates(message as StatusResponse)) as StatusResponse; // ignore: deprecated_member_use
  $pb.BuilderInfo get info_ => _i;
  @$core.pragma('dart2js:noInline')
  static StatusResponse create() => StatusResponse._();
  StatusResponse createEmptyInstance() => create();
  static $pb.PbList<StatusResponse> createRepeated() => $pb.PbList<StatusResponse>();
  @$core.pragma('dart2js:noInline')
  static StatusResponse getDefault() => _defaultInstance ??= $pb.GeneratedMessage.$_defaultFor<StatusResponse>(create);
  static StatusResponse? _defaultInstance;

  @$pb.TagNumber(1)
  $core.String get message => $_getSZ(0);
  @$pb.TagNumber(1)
  set message($core.String v) { $_setString(0, v); }
  @$pb.TagNumber(1)
  $core.bool hasMessage() => $_has(0);
  @$pb.TagNumber(1)
  void clearMessage() => clearField(1);

  @$pb.TagNumber(2)
  StatusCode get code => $_getN(1);
  @$pb.TagNumber(2)
  set code(StatusCode v) { setField(2, v); }
  @$pb.TagNumber(2)
  $core.bool hasCode() => $_has(1);
  @$pb.TagNumber(2)
  void clearCode() => clearField(2);
}

