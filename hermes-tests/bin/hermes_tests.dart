import 'package:firebase_dart/firebase_dart.dart';
import 'package:hermes_tests/di/injection.dart';

Future<void> main(List<String> arguments) async {
  FirebaseDart.setup();
  configureDependencies('dev');
}
