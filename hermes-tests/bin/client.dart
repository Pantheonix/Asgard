import 'package:hermes_tests/api/client/hermes_grpc_client.dart';
import 'package:hermes_tests/api/core/hermes.pb.dart';
import 'package:hermes_tests/di/config/config.dart';
import 'package:hermes_tests/di/config/server_config.dart';
import 'package:hermes_tests/domain/core/file_log_output.dart';
import 'package:logger/logger.dart';

Future<void> main(List<String> arguments) async {
  final config = Config.fromEnv('HERMES_CONFIG');
  final serverConfig = ServerConfig.fromJson(config.dev);

  final logger = Logger(
    output: FileLogOutput(
      serverConfig.logOutputFilePath,
    ),
  );

  final client = HermesGrpcClient.fromConfig(
    serverConfig,
    logger,
  );

  // upload
  // final testPath = 'temp/test/archived/marsx/1.zip';
  // for (int i = 1; i <= 10; i++) {
  //   final response = await client.uploadTest(
  //     testPath,
  //     Metadata()
  //       ..problemId = 'dyson'
  //       ..testId = '$i'
  //       ..testSize = File(testPath).lengthSync(),
  //   );
  //   logger.d(response);
  // }

  // download
  // for (int i = 1; i <= 10; i++) {
  //   final response = await client.downloadTest(
  //     DownloadRequest()
  //       ..problemId = 'dyson'
  //       ..testId = '$i',
  //   );
  //   logger.d(response);
  // }

  // delete
  // final response = await client.deleteTest(
  //   DeleteTestRequest()
  //     ..problemId = 'stardust'
  //     ..testId = '10',
  // );
  // logger.d(response);

  // get download link
  final response = await client.getDownloadLinkForTest(
    GetDownloadLinkForTestRequest()
      ..problemId = 'stardust'
      ..testId = '9',
  );
  logger.d(response);
  print(response);

  await client.close();
}
