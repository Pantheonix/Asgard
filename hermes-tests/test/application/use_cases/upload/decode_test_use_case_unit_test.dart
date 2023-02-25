import 'dart:io';

import 'package:cqrs_mediator/cqrs_mediator.dart';
import 'package:dartz/dartz.dart';
import 'package:hermes_tests/application/use_cases/upload/decode_test_use_case.dart';
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

  group('Decode Test UseCase Unit Tests', () {
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
        () => DecodeTestAsyncQueryHandler(
          logger,
        ),
      );

      sut = Mediator.instance;
    });

    test(
        'Given metadata for archived test existing on disk, '
        'When decode test use case is called, '
        'Then metadata for corresponding unarchived test is returned',
        () async {
      // Arrange
      final TestMetadata testMetadata = TestMetadata.testToDecode(
        problemId: 'marsx',
        testId: '1',
        archiveTypeExtension: testConfig.archiveTypeExtension,
        fromDir: testConfig.tempLocalArchivedTestFolder,
        toDir: testConfig.tempLocalUnarchivedTestFolder,
        inputFilename: testConfig.inputFilename,
        outputFilename: testConfig.outputFilename,
      );

      // Act
      final Either<StorageFailure, Unit> result = await sut.run(
        DecodeTestAsyncQuery(
          testMetadata: testMetadata,
        ),
      );

      // Assert
      expect(result.isRight(), true);

      expect(
        File(
          '${testConfig.tempLocalUnarchivedTestFolder}/${testMetadata.problemId}/${testMetadata.testId}/${testConfig.inputFilename}',
        ).existsSync(),
        true,
      );
      expect(
        File(
          '${testConfig.tempLocalUnarchivedTestFolder}/${testMetadata.problemId}/${testMetadata.testId}/${testConfig.outputFilename}',
        ).existsSync(),
        true,
      );

      _disposeLocalDirectory(
        '${testConfig.tempLocalUnarchivedTestFolder}/${testMetadata.problemId}/${testMetadata.testId}',
      );
    });

    test(
        'Given metadata for archived test not existing on disk, '
        'When decode test use case is called, '
        'Then localTestNotFound storage failure is returned', () async {
      // Arrange
      final TestMetadata testMetadata = TestMetadata.testToDecode(
        problemId: 'marsx',
        testId: '3',
        archiveTypeExtension: testConfig.archiveTypeExtension,
        fromDir: testConfig.tempLocalArchivedTestFolder,
        toDir: testConfig.tempLocalUnarchivedTestFolder,
        inputFilename: testConfig.inputFilename,
        outputFilename: testConfig.outputFilename,
      );

      // Act
      final Either<StorageFailure, Unit> result = await sut.run(
        DecodeTestAsyncQuery(
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
        'Given metadata for non-zip or tampered archived test, '
        'When decode test use case is called, '
        'Then invalidLocalTestFormat storage failure is returned', () async {
      // Arrange
      final TestMetadata testMetadata = TestMetadata.testToDecode(
        problemId: 'marsx',
        testId: '4',
        archiveTypeExtension: testConfig.archiveTypeExtension,
        fromDir: testConfig.tempLocalArchivedTestFolder,
        toDir: testConfig.tempLocalUnarchivedTestFolder,
        inputFilename: testConfig.inputFilename,
        outputFilename: testConfig.outputFilename,
      );

      // Act
      final Either<StorageFailure, Unit> result = await sut.run(
        DecodeTestAsyncQuery(
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
        'When decode test use case is called, '
        'Then unexpected storage failure is returned', () async {
      // Arrange
      final TestMetadata testMetadata = TestMetadata.testToEncode(
        problemId: 'marsx',
        testId: '4',
        archiveTypeExtension: testConfig.archiveTypeExtension,
        fromDir: testConfig.tempLocalArchivedTestFolder,
        toDir: testConfig.tempLocalUnarchivedTestFolder,
        inputFilename: testConfig.inputFilename,
        outputFilename: testConfig.outputFilename,
      );

      // Act
      final Either<StorageFailure, Unit> result = await sut.run(
        DecodeTestAsyncQuery(
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

void _disposeLocalDirectory(String path) {
  final Directory dir = Directory(path);
  if (dir.existsSync()) {
    dir.deleteSync(recursive: true);
  }
}
