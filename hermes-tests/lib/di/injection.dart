import 'package:get_it/get_it.dart';
import 'package:hermes_tests/di/injection.config.dart';
import 'package:injectable/injectable.dart';

final getIt = GetIt.instance;

@injectableInit
void configureDependencies(String env) => getIt.init(environment: env);