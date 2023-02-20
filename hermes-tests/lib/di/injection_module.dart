import 'package:firebase_dart/firebase_dart.dart';
import 'package:hermes_tests/di/config/config.dart';
import 'package:hermes_tests/di/config/firebase_config.dart';
import 'package:injectable/injectable.dart';

@module
abstract class InjectionModule {
  @lazySingleton
  Config get config => Config.fromJsonFile('config.json');

  @lazySingleton
  FirebaseConfig get firebaseConfig => FirebaseConfig.fromJson(config.firebase);

  @lazySingleton
  Future<FirebaseApp> get firebaseApp async => await Firebase.initializeApp(
        options: FirebaseOptions.fromMap(config.firebase),
      );

  @lazySingleton
  Future<FirebaseStorage> get firebaseStorage async =>
      FirebaseStorage.instanceFor(app: await firebaseApp);
}
