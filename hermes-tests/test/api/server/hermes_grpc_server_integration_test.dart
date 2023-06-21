import 'dart:io';

import 'package:dartz/dartz.dart';
import 'package:firebase_dart/firebase_dart.dart';
import 'package:hermes_tests/api/client/hermes_grpc_client.dart';
import 'package:hermes_tests/api/core/hermes.pb.dart';
import 'package:hermes_tests/api/server/hermes_grpc_server.dart';
import 'package:hermes_tests/di/config/server_config.dart';
import 'package:hermes_tests/di/injection.dart';
import 'package:hermes_tests/domain/core/file_manager.dart';
import 'package:logger/logger.dart';
import 'package:test/test.dart';

late final ServerConfig testConfig;
late final FirebaseStorage storage;
late HermesGrpcClient client;
late final HermesGrpcServer server;

void main() {
  group('HermesGrpcServer Integration Test', () {
    setUpAll(() async {
      FirebaseDart.setup();
      await configureDependencies('test');

      testConfig = getIt<ServerConfig>();
      storage = await getIt.getAsync<FirebaseStorage>();
      final logger = getIt.get<Logger>();

      server = HermesGrpcServer(
        testConfig,
        mediator,
        logger,
      );

      server.start();
    });

    setUp(() async {
      final logger = getIt.get<Logger>();

      client = HermesGrpcClient.fromConfig(
        testConfig,
        logger,
      );
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

      // Act
      final UploadResponse response = await client.uploadTest(
        testPath,
        testMetadata,
      );

      // Assert
      expect(response.status.code, StatusCode.Ok);

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
          '${testConfig.tempLocalArchivedTestFolder}/${testMetadata.problemId}/${testMetadata.testId}.zip';
      final String localTestPath =
          '${testConfig.tempLocalUnarchivedTestFolder}/${testMetadata.problemId}/${testMetadata.testId}';

      FileManager.disposeLocalFile(localTestArchivePath);
      FileManager.disposeLocalDirectory(localTestPath);

      await FileManager.disposeRemoteAsset(storage, remoteTestInputPath);
      await FileManager.disposeRemoteAsset(storage, remoteTestOutputPath);

      client.close();
    });

    test(
        'Given grpc client requests given test to be downloaded, '
        'When download rpc service method is called on the server-side, '
        'Then the downloaded test archive is accessible from the local filesystem',
        () async {
      // Arrange
      final request = DownloadRequest()
        ..problemId = 'marsx'
        ..testId = '9';

      // Act
      final Tuple2<StatusResponse, String> response =
          await client.downloadTest(request);

      // Assert
      expect(response.value1.code, StatusCode.Ok);

      final downloadedTestArchive = File(response.value2);
      expect(downloadedTestArchive.existsSync(), true);

      final String localTestArchivePath =
          '${testConfig.tempLocalArchivedTestFolder}/${request.problemId}/${request.testId}.zip';
      final String localTestPath =
          '${testConfig.tempLocalUnarchivedTestFolder}/${request.problemId}/${request.testId}';
      final String localTestClientDownloadPath =
          '${testConfig.tempLocalArchivedTestFolder}/${request.problemId}/${request.testId}-downloaded.zip';

      FileManager.disposeLocalFile(localTestArchivePath);
      FileManager.disposeLocalDirectory(localTestPath);
      FileManager.disposeLocalFile(localTestClientDownloadPath);

      client.close();
    });

    test(
        'Given grpc client requests to delete given test, '
        'When delete rpc service method is called on the server-side, '
        'Then the test is deleted from the remote firebase cloud storage',
        () async {
      // Arrange
      final String testPath = 'temp/test/archived/marsx/1-valid.zip';
      final Metadata testMetadata = Metadata()
        ..problemId = 'marsx'
        ..testId = '10'
        ..testSize = File(testPath).lengthSync();

      await client.uploadTest(
        testPath,
        testMetadata,
      );

      final DeleteTestRequest request = DeleteTestRequest()
        ..problemId = testMetadata.problemId
        ..testId = testMetadata.testId;

      // Act
      final DeleteTestResponse response = await client.deleteTest(request);

      // Assert
      expect(response.status.code, StatusCode.Ok);

      client.close();
    });
  });
}
