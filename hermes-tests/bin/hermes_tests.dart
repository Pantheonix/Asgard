import 'dart:io';

import 'package:firebase_dart/firebase_dart.dart';
import 'package:hermes_tests/api/server/hermes_grpc_server.dart';
import 'package:hermes_tests/di/config/server_config.dart';
import 'package:hermes_tests/di/injection.dart';
import 'package:logger/logger.dart';

Future<void> main(List<String> arguments) async {
  FirebaseDart.setup();
  await configureDependencies('dev');

  final config = getIt<ServerConfig>();
  final logger = getIt<Logger>();

  final hermesServer = HermesGrpcServer(
    config,
    mediator,
    logger,
  );
  await hermesServer.start();
  print('Server listening on port ${config.port}...');

  ProcessSignal.sigint.watch().listen((signal) async {
    print('Exiting...');
    await hermesServer.close().then((_) => print('Server closed'));
    exit(0);
  });
}
