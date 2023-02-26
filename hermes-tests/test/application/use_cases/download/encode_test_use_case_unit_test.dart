import 'dart:io';

import 'package:cqrs_mediator/cqrs_mediator.dart';
import 'package:dartz/dartz.dart';
import 'package:hermes_tests/application/use_cases/download/encode_test_use_case.dart';
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

  group('Encode Test UseCase Unit Tests', () {
    setUpAll(() {
      testConfig = ServerConfig.fromJson(
        Config.fromJsonFile('config.json').test,
      );
      final logger = Logger(
        output: FileLogOutput(
          testConfig.logOutputFilePath,
        ),
      );

      Mediator.instance.registerHandler(
        () => EncodeTestAsyncQueryHandler(
          logger,
        ),
      );

      sut = Mediator.instance;
    });

    test(
        'Given metadata for unarchived test existing on disk and associated archived test not existing, '
        'When encode test use case is called, '
        'Then metadata for corresponding archived test is returned', () async {
      // Arrange
      final TestMetadata testMetadata = TestMetadata.testToEncode(
        problemId: 'marsx',
        testId: '2',
        archiveTypeExtension: testConfig.archiveTypeExtension,
        fromDir: testConfig.tempLocalUnarchivedTestFolder,
        toDir: testConfig.tempLocalArchivedTestFolder,
        inputFilename: testConfig.inputFilename,
        outputFilename: testConfig.outputFilename,
      );

      // Act
      final Either<StorageFailure, Unit> result = await sut.run(
        EncodeTestAsyncQuery(
          testMetadata: testMetadata,
        ),
      );

      // Assert
      expect(result.isRight(), true);

      final archivedTestFilePath =
          '${testConfig.tempLocalArchivedTestFolder}/${testMetadata.problemId}/${testMetadata.testId}.${testConfig.archiveTypeExtension}';
      expect(
        File(archivedTestFilePath).existsSync(),
        true,
      );

      FileManager.disposeLocalFile(archivedTestFilePath);
    });

    test(
        'Given metadata for unarchived test not existing on disk and associated archived test already existing, '
        'When encode test use case is called, '
        'Then metadata for corresponding archived test is returned', () async {
      // Arrange
      final TestMetadata testMetadata = TestMetadata.testToEncode(
        problemId: 'marsx',
        testId: '1',
        archiveTypeExtension: testConfig.archiveTypeExtension,
        fromDir: testConfig.tempLocalUnarchivedTestFolder,
        toDir: testConfig.tempLocalArchivedTestFolder,
        inputFilename: testConfig.inputFilename,
        outputFilename: testConfig.outputFilename,
      );

      // Act
      final Either<StorageFailure, Unit> result = await sut.run(
        EncodeTestAsyncQuery(
          testMetadata: testMetadata,
        ),
      );

      // Assert
      expect(result.isRight(), true);
    });

    test(
        'Given metadata for unarchived test not existing on disk and associated archived test not existing too, '
        'When encode test use case is called, '
        'Then localTestNotFound storage failure is returned', () async {
      // Arrange
      final TestMetadata testMetadata = TestMetadata.testToEncode(
        problemId: 'marsx',
        testId: '3',
        archiveTypeExtension: testConfig.archiveTypeExtension,
        fromDir: testConfig.tempLocalUnarchivedTestFolder,
        toDir: testConfig.tempLocalArchivedTestFolder,
        inputFilename: testConfig.inputFilename,
        outputFilename: testConfig.outputFilename,
      );

      // Act
      final Either<StorageFailure, Unit> result = await sut.run(
        EncodeTestAsyncQuery(
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
        'Given metadata for invalid unarchived test existing on disk and associated archived test not existing, '
        'When encode test use case is called, '
        'Then invalidLocalTestFormat storage failure is returned', () async {
      // Arrange
      final TestMetadata testMetadata = TestMetadata.testToEncode(
        problemId: 'marsx',
        testId: '6',
        archiveTypeExtension: testConfig.archiveTypeExtension,
        fromDir: testConfig.tempLocalUnarchivedTestFolder,
        toDir: testConfig.tempLocalArchivedTestFolder,
        inputFilename: testConfig.inputFilename,
        outputFilename: testConfig.outputFilename,
      );

      // Act
      final Either<StorageFailure, Unit> result = await sut.run(
        EncodeTestAsyncQuery(
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
        'When encode test use case is called, '
        'Then unexpected storage failure is returned', () async {
      // Arrange
      final TestMetadata testMetadata = TestMetadata.testToDecode(
        problemId: 'marsx',
        testId: '6',
        archiveTypeExtension: testConfig.archiveTypeExtension,
        fromDir: testConfig.tempLocalUnarchivedTestFolder,
        toDir: testConfig.tempLocalArchivedTestFolder,
        inputFilename: testConfig.inputFilename,
        outputFilename: testConfig.outputFilename,
      );

      // Act
      final Either<StorageFailure, Unit> result = await sut.run(
        EncodeTestAsyncQuery(
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
