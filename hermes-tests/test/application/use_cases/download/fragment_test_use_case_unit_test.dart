import 'dart:io';

import 'package:cqrs_mediator/cqrs_mediator.dart';
import 'package:dartz/dartz.dart';
import 'package:hermes_tests/api/core/hermes.pb.dart';
import 'package:hermes_tests/application/use_cases/download/fragment_test_use_case.dart';
import 'package:hermes_tests/di/config/config.dart';
import 'package:hermes_tests/di/config/server_config.dart';
import 'package:hermes_tests/domain/core/file_log_output.dart';
import 'package:hermes_tests/domain/entities/test_metadata.dart';
import 'package:hermes_tests/domain/exceptions/storage_failures.dart';
import 'package:logger/logger.dart';
import 'package:test/test.dart';

void main() {
  late final ServerConfig testConfig;
  late final Mediator sut;

  group('Fragment Test UseCase Unit Tests', () {
    setUpAll(() {
      testConfig = ServerConfig.fromJson(
        Config.fromEnv('HERMES_CONFIG').test,
      );
      final logger = Logger(
        output: FileLogOutput(
          testConfig.logOutputFilePath,
        ),
      );

      Mediator.instance.registerHandler(
        () => FragmentTestAsyncQueryHandler(
          logger,
        ),
      );

      sut = Mediator.instance;
    });

    test(
        'Given metadata of archived test, '
        'When fragment test use case is called, '
        'Then a stream of chunks is returned', () async {
      // Arrange
      final TestMetadata testMetadata = TestMetadata.testToFragment(
        problemId: 'sum',
        testId: '1',
        archiveTypeExtension: testConfig.archiveTypeExtension,
        fromDir: testConfig.tempLocalArchivedTestFolder,
      );

      // Act
      final Either<StorageFailure, Tuple2<Stream<Chunk>, int>> result =
          await sut.run(
        FragmentTestAsyncQuery(
          testMetadata: testMetadata,
        ),
      );

      // Assert
      result.fold(
        (f) => expect(true, false),
        (responseTuple) async {
          final chunkStream = responseTuple.value1;
          expect(chunkStream, isNotNull);

          final expectedSize = File(
            '${testConfig.tempLocalArchivedTestFolder}/${testMetadata.problemId}/${testMetadata.testId}.${testConfig.archiveTypeExtension}',
          ).lengthSync();
          final actualSize = responseTuple.value2;
          expect(actualSize, expectedSize);
        },
      );
    });

    test(
        'Given metadata of archived test not existing on disk, '
        'When fragment test use case is called, '
        'Then localTestNotFound storage failure is returned', () async {
      // Arrange
      final TestMetadata testMetadata = TestMetadata.testToFragment(
        problemId: 'sum',
        testId: '7',
        archiveTypeExtension: testConfig.archiveTypeExtension,
        fromDir: testConfig.tempLocalArchivedTestFolder,
      );

      // Act
      final Either<StorageFailure, Tuple2<Stream<Chunk>, int>> result =
          await sut.run(
        FragmentTestAsyncQuery(
          testMetadata: testMetadata,
        ),
      );

      // Assert
      result.fold(
        (f) => f.maybeMap(
          localTestNotFound: (_) => expect(true, true),
          orElse: () => expect(true, false),
        ),
        (_) => expect(true, false),
      );
    });

    test(
        'Given metadata of non-zip or tampered archived test, '
        'When fragment test use case is called, '
        'Then invalidLocalTestFormat storage failure is returned', () async {
      // Arrange
      final TestMetadata testMetadata = TestMetadata.testToFragment(
        problemId: 'sum',
        testId: '4',
        archiveTypeExtension: testConfig.archiveTypeExtension,
        fromDir: testConfig.tempLocalArchivedTestFolder,
      );

      // Act
      final Either<StorageFailure, Tuple2<Stream<Chunk>, int>> result =
          await sut.run(
        FragmentTestAsyncQuery(
          testMetadata: testMetadata,
        ),
      );

      // Assert
      result.fold(
        (f) => f.maybeMap(
          invalidLocalTestFormat: (_) => expect(true, true),
          orElse: () => expect(true, false),
        ),
        (_) => expect(true, false),
      );
    });

    test(
        'Given invalid test metadata, '
        'When fragment test use case is called, '
        'Then unexpected storage failure is returned', () async {
      // Arrange
      final TestMetadata testMetadata = TestMetadata.testToUpload(
        problemId: 'sum',
        testId: '4',
        fromDir: testConfig.tempLocalUnarchivedTestFolder,
        toDir: testConfig.remoteUnarchivedTestFolder,
        inputFilename: testConfig.inputFilename,
        outputFilename: testConfig.outputFilename,
      );

      // Act
      final Either<StorageFailure, Tuple2<Stream<Chunk>, int>> result =
          await sut.run(
        FragmentTestAsyncQuery(
          testMetadata: testMetadata,
        ),
      );

      // Assert
      result.fold(
        (f) => f.maybeMap(
          unexpected: (_) => expect(true, true),
          orElse: () => expect(true, false),
        ),
        (_) => expect(true, false),
      );
    });
  });
}
