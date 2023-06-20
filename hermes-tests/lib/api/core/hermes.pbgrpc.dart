///
//  Generated code. Do not modify.
//  source: hermes.proto
//
// @dart = 2.12
// ignore_for_file: annotate_overrides,camel_case_types,constant_identifier_names,directives_ordering,library_prefixes,non_constant_identifier_names,prefer_final_fields,return_of_invalid_type,unnecessary_const,unnecessary_import,unnecessary_this,unused_import,unused_shown_name

import 'dart:async' as $async;

import 'dart:core' as $core;

import 'package:grpc/service_api.dart' as $grpc;
import 'hermes.pb.dart' as $0;
export 'hermes.pb.dart';

class HermesTestsServiceClient extends $grpc.Client {
  static final _$uploadTest =
      $grpc.ClientMethod<$0.UploadRequest, $0.UploadResponse>(
          '/asgard.hermes.HermesTestsService/UploadTest',
          ($0.UploadRequest value) => value.writeToBuffer(),
          ($core.List<$core.int> value) => $0.UploadResponse.fromBuffer(value));
  static final _$downloadTest =
      $grpc.ClientMethod<$0.DownloadRequest, $0.DownloadResponse>(
          '/asgard.hermes.HermesTestsService/DownloadTest',
          ($0.DownloadRequest value) => value.writeToBuffer(),
          ($core.List<$core.int> value) =>
              $0.DownloadResponse.fromBuffer(value));
  static final _$getDownloadLinkForTest = $grpc.ClientMethod<
          $0.GetDownloadLinkForTestRequest, $0.GetDownloadLinkForTestResponse>(
      '/asgard.hermes.HermesTestsService/GetDownloadLinkForTest',
      ($0.GetDownloadLinkForTestRequest value) => value.writeToBuffer(),
      ($core.List<$core.int> value) =>
          $0.GetDownloadLinkForTestResponse.fromBuffer(value));
  static final _$deleteTest =
      $grpc.ClientMethod<$0.DeleteTestRequest, $0.DeleteTestResponse>(
          '/asgard.hermes.HermesTestsService/DeleteTest',
          ($0.DeleteTestRequest value) => value.writeToBuffer(),
          ($core.List<$core.int> value) =>
              $0.DeleteTestResponse.fromBuffer(value));

  HermesTestsServiceClient($grpc.ClientChannel channel,
      {$grpc.CallOptions? options,
      $core.Iterable<$grpc.ClientInterceptor>? interceptors})
      : super(channel, options: options, interceptors: interceptors);

  $grpc.ResponseFuture<$0.UploadResponse> uploadTest(
      $async.Stream<$0.UploadRequest> request,
      {$grpc.CallOptions? options}) {
    return $createStreamingCall(_$uploadTest, request, options: options).single;
  }

  $grpc.ResponseStream<$0.DownloadResponse> downloadTest(
      $0.DownloadRequest request,
      {$grpc.CallOptions? options}) {
    return $createStreamingCall(
        _$downloadTest, $async.Stream.fromIterable([request]),
        options: options);
  }

  $grpc.ResponseFuture<$0.GetDownloadLinkForTestResponse>
      getDownloadLinkForTest($0.GetDownloadLinkForTestRequest request,
          {$grpc.CallOptions? options}) {
    return $createUnaryCall(_$getDownloadLinkForTest, request,
        options: options);
  }

  $grpc.ResponseFuture<$0.DeleteTestResponse> deleteTest(
      $0.DeleteTestRequest request,
      {$grpc.CallOptions? options}) {
    return $createUnaryCall(_$deleteTest, request, options: options);
  }
}

abstract class HermesTestsServiceBase extends $grpc.Service {
  $core.String get $name => 'asgard.hermes.HermesTestsService';

  HermesTestsServiceBase() {
    $addMethod($grpc.ServiceMethod<$0.UploadRequest, $0.UploadResponse>(
        'UploadTest',
        uploadTest,
        true,
        false,
        ($core.List<$core.int> value) => $0.UploadRequest.fromBuffer(value),
        ($0.UploadResponse value) => value.writeToBuffer()));
    $addMethod($grpc.ServiceMethod<$0.DownloadRequest, $0.DownloadResponse>(
        'DownloadTest',
        downloadTest_Pre,
        false,
        true,
        ($core.List<$core.int> value) => $0.DownloadRequest.fromBuffer(value),
        ($0.DownloadResponse value) => value.writeToBuffer()));
    $addMethod($grpc.ServiceMethod<$0.GetDownloadLinkForTestRequest,
            $0.GetDownloadLinkForTestResponse>(
        'GetDownloadLinkForTest',
        getDownloadLinkForTest_Pre,
        false,
        false,
        ($core.List<$core.int> value) =>
            $0.GetDownloadLinkForTestRequest.fromBuffer(value),
        ($0.GetDownloadLinkForTestResponse value) => value.writeToBuffer()));
    $addMethod($grpc.ServiceMethod<$0.DeleteTestRequest, $0.DeleteTestResponse>(
        'DeleteTest',
        deleteTest_Pre,
        false,
        false,
        ($core.List<$core.int> value) => $0.DeleteTestRequest.fromBuffer(value),
        ($0.DeleteTestResponse value) => value.writeToBuffer()));
  }

  $async.Stream<$0.DownloadResponse> downloadTest_Pre($grpc.ServiceCall call,
      $async.Future<$0.DownloadRequest> request) async* {
    yield* downloadTest(call, await request);
  }

  $async.Future<$0.GetDownloadLinkForTestResponse> getDownloadLinkForTest_Pre(
      $grpc.ServiceCall call,
      $async.Future<$0.GetDownloadLinkForTestRequest> request) async {
    return getDownloadLinkForTest(call, await request);
  }

  $async.Future<$0.DeleteTestResponse> deleteTest_Pre($grpc.ServiceCall call,
      $async.Future<$0.DeleteTestRequest> request) async {
    return deleteTest(call, await request);
  }

  $async.Future<$0.UploadResponse> uploadTest(
      $grpc.ServiceCall call, $async.Stream<$0.UploadRequest> request);
  $async.Stream<$0.DownloadResponse> downloadTest(
      $grpc.ServiceCall call, $0.DownloadRequest request);
  $async.Future<$0.GetDownloadLinkForTestResponse> getDownloadLinkForTest(
      $grpc.ServiceCall call, $0.GetDownloadLinkForTestRequest request);
  $async.Future<$0.DeleteTestResponse> deleteTest(
      $grpc.ServiceCall call, $0.DeleteTestRequest request);
}
