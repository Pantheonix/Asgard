import 'package:cqrs_mediator/cqrs_mediator.dart';
import 'package:get_it/get_it.dart';
import 'package:hermes_tests/application/use_cases/decode_test_use_case.dart';
import 'package:hermes_tests/application/use_cases/defragment_test_use_case.dart';
import 'package:hermes_tests/application/use_cases/upload_test_use_case.dart';
import 'package:hermes_tests/di/injection.config.dart';
import 'package:hermes_tests/domain/interfaces/i_test_repository.dart';
import 'package:injectable/injectable.dart';

final getIt = GetIt.instance;
final mediator = Mediator.instance;

@injectableInit
Future<void> configureDependencies(String env) async {
  getIt.init(environment: env);
  mediator.registerHandler(() => DefragmentTestAsyncQueryHandler());
  mediator.registerHandler(() => DecodeTestAsyncQueryHandler());

  final testRepository = await getIt.getAsync<ITestRepository>();
  mediator.registerHandler(
    () => UploadTestAsyncQueryHandler(testRepository),
  );
}
