import 'dart:async';
import 'dart:io';

import 'package:grpc/grpc.dart';
import 'package:hermes_tests/api/core/hermes.pbgrpc.dart' as hermes;
import 'package:hermes_tests/di/config/server_config.dart';

class HermesGrpcClient {
  late final ClientChannel _channel;
  late final hermes.HermesTestsServiceClient _client;

  HermesGrpcClient.fromConfig(ServerConfig config) {
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

    final Stream<List<int>> fileDataStream = file.openRead();
    final hermes.UploadRequest request = hermes.UploadRequest()
      ..metadata = testMetadata;

    requestStreamController.add(request);

    fileDataStream.listen(
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

  Future<void> close() async {
    await _channel.shutdown();
  }
}
