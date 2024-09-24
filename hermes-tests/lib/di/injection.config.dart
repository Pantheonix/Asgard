// GENERATED CODE - DO NOT MODIFY BY HAND

// **************************************************************************
// InjectableConfigGenerator
// **************************************************************************

// ignore_for_file: unnecessary_lambdas
// ignore_for_file: lines_longer_than_80_chars
// coverage:ignore-file

// ignore_for_file: no_leading_underscores_for_library_prefixes
import 'package:firebase_dart/firebase_dart.dart' as _i4;
import 'package:get_it/get_it.dart' as _i1;
import 'package:hermes_tests/di/config/config.dart' as _i3;
import 'package:hermes_tests/di/config/firebase_config.dart' as _i5;
import 'package:hermes_tests/di/injection_module.dart' as _i8;
import 'package:hermes_tests/domain/interfaces/i_test_repository.dart' as _i6;
import 'package:hermes_tests/infrastructure/repositories/test_repository.dart'
    as _i7;
import 'package:injectable/injectable.dart' as _i2;

extension GetItInjectableX on _i1.GetIt {
  // initializes the registration of main-scope dependencies inside of GetIt
  _i1.GetIt init({
    String? environment,
    _i2.EnvironmentFilter? environmentFilter,
  }) {
    final gh = _i2.GetItHelper(
      this,
      environment,
      environmentFilter,
    );
    final injectionModule = _$InjectionModule();
    gh.lazySingleton<_i3.Config>(() => injectionModule.config);
    gh.lazySingletonAsync<_i4.FirebaseApp>(() => injectionModule.firebaseApp);
    gh.lazySingleton<_i5.FirebaseConfig>(() => injectionModule.firebaseConfig);
    gh.lazySingletonAsync<_i4.FirebaseStorage>(
        () => injectionModule.firebaseStorage);
    gh.lazySingletonAsync<_i6.ITestRepository>(
        () async => _i7.TestRepository(await getAsync<_i4.FirebaseStorage>()));
    return this;
  }
}

class _$InjectionModule extends _i8.InjectionModule {}
