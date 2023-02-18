import 'dart:io';

import 'package:firebase_dart/firebase_dart.dart';
import 'package:hermes_tests/api/client/hermes_grpc_client.dart';
import 'package:hermes_tests/api/core/hermes.pb.dart';
import 'package:hermes_tests/api/server/hermes_grpc_server.dart';
import 'package:hermes_tests/di/config.dart';
import 'package:hermes_tests/di/injection.dart';
import 'package:test/test.dart';

late final Map testConfig;
late final HermesGrpcClient client;
late final HermesGrpcServer server;
late final FirebaseStorage storage;

void main() {
  group('HermesGrpcServer Integration Test', () {
    setUpAll(() async {
      FirebaseDart.setup();
      await configureDependencies('test');

      testConfig = getIt<Config>().test;
      storage = await getIt.getAsync<FirebaseStorage>();

      client = HermesGrpcClient.fromConfig(testConfig);
      server = HermesGrpcServer.fromConfig(testConfig, mediator);
    });

    test(
        'Given grpc client requests given test to be uploaded, '
        'When upload rpc service method is called on the server-side, '
        'Then the uploaded test is accessible from the remote firebase cloud storage',
        () async {
      // Arrange
      final String testPath = 'temp/test/archived/marsx/1-valid.zip';
      final Metadata testMetadata = Metadata()
        ..problemId = 'marsx'
        ..testId = '10'
        ..testSize = File(testPath).lengthSync();

      await server.start();

      // Act
      final UploadResponse response = await client.uploadTest(
        testPath,
        testMetadata,
      );

      // Assert
      expect(response.code, UploadStatusCode.Ok);

      final String remoteTestInputPath =
          'test/${testMetadata.problemId}/${testMetadata.testId}/input.txt';
      final remoteTestInputMetadata =
          await storage.ref(remoteTestInputPath).getMetadata();

      expect(remoteTestInputMetadata.fullPath, remoteTestInputPath);

      final String remoteTestOutputPath =
          'test/${testMetadata.problemId}/${testMetadata.testId}/output.txt';
      final remoteTestOutputMetadata =
          await storage.ref(remoteTestOutputPath).getMetadata();

      expect(remoteTestOutputMetadata.fullPath, remoteTestOutputPath);

      final String localTestArchivePath =
          '${testConfig['tempArchivedTestLocalPath']}/${testMetadata.problemId}/${testMetadata.testId}.zip';
      final String localTestPath =
          '${testConfig['tempUnarchivedTestLocalPath']}/${testMetadata.problemId}/${testMetadata.testId}';

      _disposeLocalFile(localTestArchivePath);
      _disposeLocalDirectory(localTestPath);

      await _disposeRemoteAsset(remoteTestInputPath);
      await _disposeRemoteAsset(remoteTestOutputPath);

      client.close();
      server.close();
    });
  });
}

Future<void> _disposeRemoteAsset(String path) async {
  await storage.ref(path).delete();
}

void _disposeLocalDirectory(String path) {
  final Directory dir = Directory(path);
  if (dir.existsSync()) {
    dir.deleteSync(recursive: true);
  }
}

void _disposeLocalFile(String path) {
  final File file = File(path);
  if (file.existsSync()) {
    file.deleteSync();
  }
}
