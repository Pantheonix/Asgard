// GENERATED CODE - DO NOT MODIFY BY HAND

// **************************************************************************
// InjectableConfigGenerator
// **************************************************************************

// ignore_for_file: no_leading_underscores_for_library_prefixes
import 'package:firebase_dart/firebase_dart.dart' as _i4;
import 'package:get_it/get_it.dart' as _i1;
import 'package:hermes_tests/di/config.dart' as _i3;
import 'package:hermes_tests/di/firebase_module.dart' as _i7;
import 'package:hermes_tests/domain/interfaces/i_test_repository.dart' as _i5;
import 'package:hermes_tests/infrastructure/repositories/test_repository.dart'
    as _i6;
import 'package:injectable/injectable.dart'
    as _i2; // ignore_for_file: unnecessary_lambdas

// ignore_for_file: lines_longer_than_80_chars
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
    final firebaseModule = _$FirebaseModule();
    gh.lazySingleton<_i3.Config>(() => firebaseModule.config);
    gh.lazySingletonAsync<_i4.FirebaseApp>(() => firebaseModule.firebaseApp);
    gh.lazySingletonAsync<_i4.FirebaseStorage>(
        () => firebaseModule.firebaseStorage);
    gh.lazySingletonAsync<_i5.ITestRepository>(
        () async => _i6.TestRepository(await getAsync<_i4.FirebaseStorage>()));
    return this;
  }
}

class _$FirebaseModule extends _i7.FirebaseModule {}
