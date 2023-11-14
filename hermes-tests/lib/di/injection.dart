import 'package:cqrs_mediator/cqrs_mediator.dart';
import 'package:get_it/get_it.dart';
import 'package:hermes_tests/application/use_cases/delete/delete_test_use_case.dart';
import 'package:hermes_tests/application/use_cases/download/download_test_use_case.dart';
import 'package:hermes_tests/application/use_cases/download/encode_test_use_case.dart';
import 'package:hermes_tests/application/use_cases/download/fragment_test_use_case.dart';
import 'package:hermes_tests/application/use_cases/get_download_link/get_download_link_for_test_use_case.dart';
import 'package:hermes_tests/application/use_cases/upload/decode_test_use_case.dart';
import 'package:hermes_tests/application/use_cases/upload/defragment_test_use_case.dart';
import 'package:hermes_tests/application/use_cases/upload/upload_test_use_case.dart';
import 'package:hermes_tests/di/config/config.dart';
import 'package:hermes_tests/di/config/server_config.dart';
import 'package:hermes_tests/di/injection.config.dart';
import 'package:hermes_tests/domain/core/file_log_output.dart';
import 'package:hermes_tests/domain/interfaces/i_test_repository.dart';
import 'package:injectable/injectable.dart';
import 'package:logger/logger.dart';

final getIt = GetIt.instance;
final mediator = Mediator.instance;

@injectableInit
Future<void> configureDependencies(String env) async {
  getIt.init(environment: env);
  final config = getIt.get<Config>();

  if (env == 'test') {
    getIt.registerLazySingleton(
      () => ServerConfig.fromJson(
        config.test,
      ),
    );
  } else if (env == 'dev') {
    getIt.registerLazySingleton(
      () => ServerConfig.fromJson(
        config.dev,
      ),
    );
  }

  final serverConfig = getIt.get<ServerConfig>();

  final logger = Logger(
    filter: ProductionFilter(),
    output: FileLogOutput(
      serverConfig.logOutputFilePath,
    ),
  );

  getIt.registerLazySingleton<Logger>(() => logger);

  mediator.registerHandler(
    () => DefragmentTestAsyncQueryHandler(
      logger,
    ),
  );
  mediator.registerHandler(
    () => FragmentTestAsyncQueryHandler(
      logger,
    ),
  );

  mediator.registerHandler(
    () => DecodeTestAsyncQueryHandler(
      logger,
    ),
  );
  mediator.registerHandler(
    () => EncodeTestAsyncQueryHandler(
      logger,
    ),
  );

  final testRepository = await getIt.getAsync<ITestRepository>();
  mediator.registerHandler(
    () => UploadTestAsyncQueryHandler(
      testRepository,
      logger,
    ),
  );
  mediator.registerHandler(
    () => DownloadTestAsyncQueryHandler(
      testRepository,
      logger,
    ),
  );
  mediator.registerHandler(
    () => DeleteTestAsyncQueryHandler(
      testRepository,
      logger,
    ),
  );
  mediator.registerHandler(
    () => GetDownloadLinkForTestAsyncQueryHandler(
      testRepository,
      logger,
    ),
  );
}
