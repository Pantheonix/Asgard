import 'dart:io';

import 'package:firebase_dart/firebase_dart.dart';
import 'package:hermes_tests/api/server/hermes_grpc_server.dart';
import 'package:hermes_tests/di/config/server_config.dart';
import 'package:hermes_tests/di/injection.dart';
import 'package:logger/logger.dart';

// TODO 1: add better test organization in test folder in order to easily distinguish between UseCase-InputTests relationships
// TODO 2: refactor TestMetadata to reflect every possible DTO stage (stream, archive, unarchive, remote)
// TODO 3: declare a static helper class for file management and centralize all file operations in it
// TODO 4: add chunking logic in FragmentTestUseCase tweaked by a given chunk size
// TODO 5: check file content, not just metadata in integration tests

Future<void> main(List<String> arguments) async {
  FirebaseDart.setup();
  late final HermesGrpcServer hermesServer;

  try {
    await configureDependencies('dev');

    final config = getIt<ServerConfig>();
    final logger = getIt<Logger>();

    hermesServer = HermesGrpcServer(
      config,
      mediator,
      logger,
    );
    await hermesServer.start();
  } catch (e) {
    print(e);
  } finally {
    ProcessSignal.sigint.watch().listen((ProcessSignal signal) async {
      print("Exiting...");
      await hermesServer.close().then(
            (_) => print("Server shutdown."),
          );
      exit(0);
    });
  }
}
