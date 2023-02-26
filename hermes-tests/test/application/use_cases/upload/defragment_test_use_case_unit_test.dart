import 'dart:async';
import 'dart:io';

import 'package:cqrs_mediator/cqrs_mediator.dart';
import 'package:dartz/dartz.dart';
import 'package:hermes_tests/api/core/hermes.pb.dart';
import 'package:hermes_tests/application/use_cases/upload/defragment_test_use_case.dart';
import 'package:hermes_tests/di/config/config.dart';
import 'package:hermes_tests/di/config/server_config.dart';
import 'package:hermes_tests/domain/core/file_log_output.dart';
import 'package:hermes_tests/domain/core/file_manager.dart';
import 'package:hermes_tests/domain/entities/test_metadata.dart';
import 'package:hermes_tests/domain/exceptions/storage_failures.dart';
import 'package:logger/logger.dart';
import 'package:test/test.dart';

void main() {
  late final ServerConfig testConfig;
  late final Mediator sut;

  group('Defragment Test UseCase Unit Tests', () {
    setUpAll(() async {
      testConfig = ServerConfig.fromJson(
        Config.fromJsonFile('config.json').test,
      );
      final logger = Logger(
        output: FileLogOutput(
          testConfig.logOutputFilePath,
        ),
      );

      Mediator.instance.registerHandler(
        () => DefragmentTestAsyncQueryHandler(
          logger,
        ),
      );

      sut = Mediator.instance;
    });

    test(
        'Given stream of chunks of archived test, '
        'When defragment test use case is called, '
        'Then the test is successfully written on disk '
        'and associated metadata is returned', () async {
      // Arrange
      final String inputPath = 'temp/test/archived/marsx/1-valid.zip';
      final int testSize = File(inputPath).lengthSync();

      final Metadata metadata = Metadata()
        ..problemId = 'marsx'
        ..testId = '2'
        ..testSize = testSize;

      final Stream<Chunk> chunkStream =
          FileManager.readStreamOfChunksForFile(inputPath);

      final testMetadata = TestMetadata.testToDefragment(
        problemId: metadata.problemId,
        testId: metadata.testId,
        testSize: testSize,
        toDir: testConfig.tempLocalArchivedTestFolder,
        archiveTypeExtension: testConfig.archiveTypeExtension,
        chunkStream: chunkStream,
        maxTestSize: testConfig.testMaxSizeInBytes,
      );

      // Act
      final Either<StorageFailure, Unit> result = await sut.run(
        DefragmentTestAsyncQuery(
          testMetadata: testMetadata,
        ),
      );

      // Assert
      expect(result.isRight(), true);

      final archivedTestPath =
          '${testConfig.tempLocalArchivedTestFolder}/${testMetadata.problemId}/${testMetadata.testId}.${testConfig.archiveTypeExtension}';
      final File file = File(archivedTestPath);
      expect(file.existsSync(), true);
      expect(file.lengthSync(), testSize);

      FileManager.disposeLocalFile(archivedTestPath);
    });

    test(
        'Given stream of chunks of a non-zip or tampered test archive, '
        'When defragment test use case is called, '
        'Then invalidLocalTestFormat storage failure is returned', () async {
      // Arrange
      final String inputPath = 'temp/test/archived/marsx/1-invalid.tar.xz';
      final int testSize = File(inputPath).lengthSync();

      final Metadata metadata = Metadata()
        ..problemId = 'marsx'
        ..testId = '2'
        ..testSize = testSize;

      final Stream<Chunk> chunkStream =
          FileManager.readStreamOfChunksForFile(inputPath);

      final testMetadata = TestMetadata.testToDefragment(
        problemId: metadata.problemId,
        testId: metadata.testId,
        testSize: testSize,
        toDir: testConfig.tempLocalArchivedTestFolder,
        archiveTypeExtension: testConfig.archiveTypeExtension,
        chunkStream: chunkStream,
        maxTestSize: testConfig.testMaxSizeInBytes,
      );

      // Act
      final Either<StorageFailure, Unit> result = await sut.run(
        DefragmentTestAsyncQuery(
          testMetadata: testMetadata,
        ),
      );

      // Assert
      result.fold(
        (f) {
          f.maybeMap(
            invalidLocalTestFormat: (_) => expect(true, true),
            orElse: () => expect(true, false),
          );
        },
        (_) => expect(true, false),
      );
    });

    test(
        'Given stream of chunks of archived test that exceeds memory limit, '
        'When defragment test use case is called, '
        'Then testSizeLimitExceeded storage failure is returned', () async {
      // Arrange
      final String inputPath = 'temp/test/archived/marsx/1-oversize.zip';
      final int testSize = File(inputPath).lengthSync();

      final Metadata metadata = Metadata()
        ..problemId = 'marsx'
        ..testId = '2'
        ..testSize = testSize;

      final Stream<Chunk> chunkStream =
          FileManager.readStreamOfChunksForFile(inputPath);

      final testMetadata = TestMetadata.testToDefragment(
        problemId: metadata.problemId,
        testId: metadata.testId,
        testSize: testSize,
        toDir: testConfig.tempLocalArchivedTestFolder,
        archiveTypeExtension: testConfig.archiveTypeExtension,
        chunkStream: chunkStream,
        maxTestSize: testConfig.testMaxSizeInBytes,
      );

      // Act
      final Either<StorageFailure, Unit> result = await sut.run(
        DefragmentTestAsyncQuery(
          testMetadata: testMetadata,
        ),
      );

      // Assert
      result.fold(
        (f) {
          f.maybeMap(
            testSizeLimitExceeded: (_) => expect(true, true),
            orElse: () => expect(true, false),
          );
        },
        (_) => expect(true, false),
      );
    });

    test(
        'Given invalid test metadata, '
        'When defragment test use case is called, '
        'Then unexpected storage failure is returned', () async {
      // Arrange
      final String inputPath = 'temp/test/archived/marsx/1-oversize.zip';
      final int testSize = File(inputPath).lengthSync();

      final Metadata metadata = Metadata()
        ..problemId = 'marsx'
        ..testId = '2'
        ..testSize = testSize;

      final Stream<Chunk> chunkStream =
          FileManager.readStreamOfChunksForFile(inputPath);

      final testMetadata = TestMetadata.testToFragment(
        problemId: metadata.problemId,
        testId: metadata.testId,
        archiveTypeExtension: testConfig.archiveTypeExtension,
        fromDir: testConfig.tempLocalArchivedTestFolder,
      );

      // Act
      final Either<StorageFailure, Unit> result = await sut.run(
        DefragmentTestAsyncQuery(
          testMetadata: testMetadata,
        ),
      );

      // Assert
      result.fold(
        (f) {
          f.maybeMap(
            unexpected: (_) => expect(true, true),
            orElse: () => expect(true, false),
          );
        },
        (_) => expect(true, false),
      );
    });
  });
}
