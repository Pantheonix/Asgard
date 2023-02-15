import 'package:firebase_dart/firebase_dart.dart';
import 'package:hermes_tests/di/config.dart';
import 'package:injectable/injectable.dart';

@module
abstract class FirebaseModule {
  @lazySingleton
  Config get config => Config.fromJsonFile('config.json');

  @lazySingleton
  Future<FirebaseApp> get firebaseApp async => await Firebase.initializeApp(
        options: FirebaseOptions.fromMap(config.firebase),
      );

  @lazySingleton
  Future<FirebaseStorage> get firebaseStorage async =>
      FirebaseStorage.instanceFor(app: await firebaseApp);
}
