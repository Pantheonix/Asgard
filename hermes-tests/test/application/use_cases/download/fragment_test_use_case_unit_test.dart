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
        Config.fromJsonFile('config.json').test,
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
      final TestMetadata testMetadata = TestMetadata(
        problemId: 'marsx',
        testId: '1',
        srcTestRootFolder: testConfig.tempArchivedTestLocalPath,
        destTestRootFolder: testConfig.tempArchivedTestLocalPath,
      );

      // Act
      final Either<StorageFailure, Stream<Chunk>> result = await sut.run(
        FragmentTestAsyncQuery(testMetadata: testMetadata),
      );

      // Assert
      result.fold(
        (f) => expect(true, false),
        (chunkStream) async {
          expect(chunkStream, isNotNull);
          expect(chunkStream, isA<Stream<Chunk>>());

          final List<Chunk> chunks = await chunkStream.toList();
          final expectedSize = File(testMetadata.archivedTestPath).lengthSync();
          final actualSize = chunks.fold<int>(
            0,
            (previousValue, element) => previousValue + element.data.length,
          );

          expect(actualSize, expectedSize);
        },
      );
    });

    test(
        'Given metadata of archived test not existing on disk, '
        'When fragment test use case is called, '
        'Then localTestNotFound storage failure is returned', () async {
      // Arrange
      final TestMetadata testMetadata = TestMetadata(
        problemId: 'marsx',
        testId: '7',
        srcTestRootFolder: testConfig.tempArchivedTestLocalPath,
        destTestRootFolder: testConfig.tempArchivedTestLocalPath,
      );

      // Act
      final Either<StorageFailure, Stream<Chunk>> result = await sut.run(
        FragmentTestAsyncQuery(testMetadata: testMetadata),
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
      final TestMetadata testMetadata = TestMetadata(
        problemId: 'marsx',
        testId: '4',
        srcTestRootFolder: testConfig.tempArchivedTestLocalPath,
        destTestRootFolder: testConfig.tempArchivedTestLocalPath,
      );

      // Act
      final Either<StorageFailure, Stream<Chunk>> result = await sut.run(
        FragmentTestAsyncQuery(testMetadata: testMetadata),
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
