import 'dart:async';
import 'dart:io';

import 'package:grpc/grpc.dart';
import 'package:hermes_tests/api/core/hermes.pbgrpc.dart';
import 'package:hermes_tests/di/config/server_config.dart';

class HermesGrpcClient {
  late final ClientChannel channel;
  late final HermesTestsServiceClient client;

  HermesGrpcClient.fromConfig(ServerConfig config) {
    channel = ClientChannel(
      config.host,
      port: config.port,
      options: const ChannelOptions(
        credentials: ChannelCredentials.insecure(),
      ),
    );

    client = HermesTestsServiceClient(
      channel,
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
    final response = client.uploadTest(requestStreamController.stream);

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
        print('client stream closed');
      },
      onError: (error) => requestStreamController.addError(error),
      cancelOnError: true,
    );

    return response;
  }

  Future<void> close() async {
    await channel.shutdown();
  }
}
