import 'dart:io';

import 'package:cqrs_mediator/cqrs_mediator.dart';
import 'package:dartz/dartz.dart';
import 'package:hermes_tests/application/use_cases/download/encode_test_use_case.dart';
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
      final TestMetadata testMetadata = TestMetadata(
        problemId: 'marsx',
        testId: '2',
        srcTestRootFolder: testConfig.tempUnarchivedTestLocalPath,
        destTestRootFolder: testConfig.tempArchivedTestLocalPath,
      );

      // Act
      final Either<StorageFailure, TestMetadata> result = await sut.run(
        EncodeTestAsyncQuery(testMetadata: testMetadata),
      );

      // Assert
      result.fold(
        (f) => expect(true, false),
        (actualTestMetadata) {
          final TestMetadata expectedTestMetadata = testMetadata.copyWith(
            srcTestRootFolder: testConfig.tempArchivedTestLocalPath,
            destTestRootFolder: testConfig.tempArchivedTestLocalPath,
          );

          expect(actualTestMetadata, expectedTestMetadata);
          expect(File(testMetadata.archivedTestPath).existsSync(), true);

          _disposeLocalFile(testMetadata.archivedTestPath);
        },
      );
    });

    test(
        'Given metadata for unarchived test not existing on disk and associated archived test already existing, '
        'When encode test use case is called, '
        'Then metadata for corresponding archived test is returned', () async {
      // Arrange
      final TestMetadata testMetadata = TestMetadata(
        problemId: 'marsx',
        testId: '1',
        srcTestRootFolder: testConfig.tempUnarchivedTestLocalPath,
        destTestRootFolder: testConfig.tempArchivedTestLocalPath,
      );

      // Act
      final Either<StorageFailure, TestMetadata> result = await sut.run(
        EncodeTestAsyncQuery(testMetadata: testMetadata),
      );

      // Assert
      result.fold(
        (f) => expect(true, false),
        (actualTestMetadata) {
          final TestMetadata expectedTestMetadata = testMetadata.copyWith(
            srcTestRootFolder: testConfig.tempArchivedTestLocalPath,
            destTestRootFolder: testConfig.tempArchivedTestLocalPath,
          );

          expect(actualTestMetadata, expectedTestMetadata);
        },
      );
    });

    test(
        'Given metadata for unarchived test not existing on disk and associated archived test not existing too, '
        'When encode test use case is called, '
        'Then localTestNotFound storage failure is returned', () async {
      // Arrange
      final TestMetadata testMetadata = TestMetadata(
        problemId: 'marsx',
        testId: '3',
        srcTestRootFolder: testConfig.tempUnarchivedTestLocalPath,
        destTestRootFolder: testConfig.tempArchivedTestLocalPath,
      );

      // Act
      final Either<StorageFailure, TestMetadata> result = await sut.run(
        EncodeTestAsyncQuery(testMetadata: testMetadata),
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
      final TestMetadata testMetadata = TestMetadata(
        problemId: 'marsx',
        testId: '6',
        srcTestRootFolder: testConfig.tempUnarchivedTestLocalPath,
        destTestRootFolder: testConfig.tempArchivedTestLocalPath,
      );

      // Act
      final Either<StorageFailure, TestMetadata> result = await sut.run(
        EncodeTestAsyncQuery(testMetadata: testMetadata),
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
  });
}

void _disposeLocalFile(String path) {
  final File file = File(path);
  if (file.existsSync()) {
    file.deleteSync();
  }
}
