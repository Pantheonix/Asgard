import 'dart:io';

import 'package:cron/cron.dart';
import 'package:firebase_dart/firebase_dart.dart';
import 'package:hermes_tests/api/server/hermes_grpc_server.dart';
import 'package:hermes_tests/di/config/server_config.dart';
import 'package:hermes_tests/di/injection.dart';
import 'package:hermes_tests/domain/core/file_manager.dart';
import 'package:logger/logger.dart';

// TODO: add chunking logic in FragmentTestUseCase tweaked by a given chunk size
// TODO: add better test organization in test folder in order to easily distinguish between UseCase-InputTests relationships
// TODO: check file content, not just metadata in integration tests
// TODO: move test metadata validation logic from use cases to domain layer

Future<void> main(List<String> arguments) async {
  FirebaseDart.setup();
  late final HermesGrpcServer hermesServer;
  
  try {
    await configureDependencies('dev');

    final config = getIt<ServerConfig>();

    // define a cron job which will delete all files
    // from temp local archived and unarchived folders

    // final everyFiveSeconds = '*/5 * * * * *';
    final everyDayAtMidnight = '0 0 * * *';

    Cron().schedule(Schedule.parse(everyDayAtMidnight), () async {
      FileManager.disposeLocalDirectoryChildren(
        config.tempLocalArchivedTestFolder,
      );
      FileManager.disposeLocalDirectoryChildren(
        config.tempLocalUnarchivedTestFolder,
      );
    });

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
    ProcessSignal.sigint.watch().listen(
      (ProcessSignal signal) async {
        print("Exiting...");
        await hermesServer.close().then(
              (_) => print("Server shutdown."),
            );
        exit(0);
      },
    );
  }
}
