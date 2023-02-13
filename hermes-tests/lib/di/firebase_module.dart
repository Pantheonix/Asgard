import 'dart:convert';
import 'dart:io';

import 'package:firebase_dart/firebase_dart.dart';
import 'package:injectable/injectable.dart';

@module
abstract class FirebaseModule {
  @lazySingleton
  Future<FirebaseApp> get firebaseApp async => await Firebase.initializeApp(
        options: FirebaseOptions.fromMap(
          jsonDecode(
            File('firebase-config.json').readAsStringSync(),
          ),
        ),
      );

  @lazySingleton
  Future<FirebaseStorage> get firebaseStorage async => FirebaseStorage.instanceFor(app: await firebaseApp);
}
