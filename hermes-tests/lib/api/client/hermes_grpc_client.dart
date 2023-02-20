import 'dart:async';
import 'dart:io';

import 'package:grpc/grpc.dart';
import 'package:hermes_tests/api/core/hermes.pbgrpc.dart';
import 'package:hermes_tests/di/config/server_config.dart';

class HermesGrpcClient {
  late final ClientChannel _channel;
  late final HermesTestsServiceClient _client;

  HermesGrpcClient.fromConfig(ServerConfig config) {
    _channel = ClientChannel(
      config.host,
      port: config.port,
      options: const ChannelOptions(
        credentials: ChannelCredentials.insecure(),
      ),
    );

    _client = HermesTestsServiceClient(
      _channel,
      options: CallOptions(
        timeout: Duration(seconds: config.timeoutInSeconds),
      ),
    );
  }

  Future<UploadResponse> uploadTest(
    String testPath,
    Metadata testMetadata,
  ) async {
    final requestStreamController = StreamController<UploadRequest>();
    final response = _client.uploadTest(requestStreamController.stream);

    final File file = File(testPath);
    if (file.existsSync() == false) {
      return Future.value(
        UploadResponse()
          ..code = UploadStatusCode.Failed
          ..message = 'Test file not found',
      );
    }

    final Stream<List<int>> fileDataStream = file.openRead();
    final UploadRequest request = UploadRequest()..metadata = testMetadata;

    requestStreamController.add(request);

    fileDataStream.listen(
      (data) => requestStreamController.add(
        UploadRequest()..chunk = (Chunk()..data = data),
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
