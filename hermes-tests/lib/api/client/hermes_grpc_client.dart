import 'dart:async';
import 'dart:io';

import 'package:dartz/dartz.dart';
import 'package:grpc/grpc.dart';
import 'package:hermes_tests/api/core/hermes.pb.dart';
import 'package:hermes_tests/api/core/hermes.pbgrpc.dart' as hermes;
import 'package:hermes_tests/di/config/server_config.dart';
import 'package:logger/logger.dart';

class HermesGrpcClient {
  late final ClientChannel _channel;
  late final hermes.HermesTestsServiceClient _client;
  final Logger _logger;

  HermesGrpcClient.fromConfig(ServerConfig config, this._logger) {
    _channel = ClientChannel(
      config.host,
      port: config.port,
      options: const ChannelOptions(
        credentials: ChannelCredentials.insecure(),
      ),
    );

    _client = hermes.HermesTestsServiceClient(
      _channel,
      options: CallOptions(
        timeout: Duration(seconds: config.timeoutInSeconds),
      ),
    );
  }

  Future<hermes.UploadResponse> uploadTest(
    String testPath,
    hermes.Metadata testMetadata,
  ) async {
    final requestStreamController = StreamController<hermes.UploadRequest>();
    final response = _client.uploadTest(requestStreamController.stream);

    final File file = File(testPath);
    if (file.existsSync() == false) {
      return Future.value(
        hermes.UploadResponse()
          ..status = (hermes.StatusResponse()
            ..code = hermes.StatusCode.Failed
            ..message = 'Test file not found'),
      );
    }

    final Stream<List<int>> outputFileSink = file.openRead();
    final hermes.UploadRequest request = hermes.UploadRequest()
      ..metadata = testMetadata;

    requestStreamController.add(request);

    outputFileSink.listen(
      (data) => requestStreamController.add(
        hermes.UploadRequest()..chunk = (hermes.Chunk()..data = data),
      ),
      onDone: () {
        requestStreamController.close();
        print('_client stream closed');
      },
      onError: (error) => requestStreamController.addError(error),
      cancelOnError: true,
    );

    return response;
  }

  Future<Tuple2<StatusResponse, String>> downloadTest(
    hermes.DownloadRequest downloadRequest,
  ) async {
    final responseStream = _client.downloadTest(downloadRequest);
    late final Metadata responseMetadata;
    late final StatusResponse responseStatus;

    final String testPath =
        'temp/test/archived/${downloadRequest.problemId}/${downloadRequest.testId}-downloaded.zip';
    final File downloadedTestFile = File(testPath);

    final IOSink outputFileSink = downloadedTestFile.openWrite();

    _logger.d('Writing to file: $testPath');

    await for (final responseItem in responseStream) {
      if (responseItem.hasMetadata()) {
        responseMetadata = responseItem.metadata;
        continue;
      }

      if (responseItem.hasStatus()) {
        responseStatus = responseItem.status;
        continue;
      }

      outputFileSink.add(responseItem.chunk.data);

      _logger.d('Chunk received: ${responseItem.chunk.data.length} bytes');
    }

    await outputFileSink.close();

    _logger.d('Downloaded test file: $testPath');

    return Tuple2(
      responseStatus,
      testPath,
    );
  }

  Future<void> close() async {
    await _channel.shutdown();
  }
}
