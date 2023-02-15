import 'dart:io';

import 'package:cqrs_mediator/cqrs_mediator.dart';
import 'package:dartz/dartz.dart';
import 'package:hermes_tests/application/use_cases/decode_test_use_case.dart';
import 'package:hermes_tests/di/config.dart';
import 'package:hermes_tests/domain/entities/test_metadata.dart';
import 'package:hermes_tests/domain/exceptions/storage_failures.dart';
import 'package:test/test.dart';

void main() {
  late final Map testConfig;
  late final Mediator sut;

  group('Decode Test UseCase Unit Tests', () {
    setUpAll(() {
      testConfig = Config.fromJsonFile('config.json').test;

      Mediator.instance.registerHandler(
        () => DecodeTestAsyncQueryHandler(),
      );

      sut = Mediator.instance;
    });

    test(
        'Given metadata for archived test existing on disk, '
        'When decode test use case is called, '
        'Then metadata for corresponding unarchived test is returned',
        () async {
      // Arrange
      final TestMetadata testMetadata = TestMetadata(
        problemId: 'marsx',
        testId: '1',
        srcTestRootFolder: testConfig['tempArchivedTestLocalPath'],
        destTestRootFolder: testConfig['tempUnarchivedTestLocalPath'],
      );

      // Act
      final Either<StorageFailure, TestMetadata> result = await sut.run(
        DecodeTestAsyncQuery(
          testMetadata: testMetadata,
          destTestRootFolderForUnarchivedTest: testConfig['tempTestRemotePath'],
        ),
      );

      // Assert
      expect(result.isRight(), true);
      expect(result.fold(id, id), isA<TestMetadata>());

      expect(File(testMetadata.destTestInputPath).existsSync(), true);
      expect(File(testMetadata.destTestOutputPath).existsSync(), true);

      _disposeLocalAsset(testMetadata.unarchivedTestPath);
    });

    test(
        'Given metadata for archived test not existing on disk, '
        'When decode test use case is called, '
        'Then storage failure is returned', () async {
      // Arrange
      final TestMetadata testMetadata = TestMetadata(
        problemId: 'marsx',
        testId: '3',
        srcTestRootFolder: testConfig['tempArchivedTestLocalPath'],
        destTestRootFolder: testConfig['tempUnarchivedTestLocalPath'],
      );

      // Act
      final Either<StorageFailure, TestMetadata> result = await sut.run(
        DecodeTestAsyncQuery(
          testMetadata: testMetadata,
          destTestRootFolderForUnarchivedTest: testConfig['tempTestRemotePath'],
        ),
      );

      // Assert
      expect(result.isLeft(), true);
      expect(result.fold(id, id), isA<StorageFailure>());

      // expect StorageFailure.invalidLocalTest
      result.fold(
        (l) => l.maybeMap(
          invalidLocalTest: (_) => expect(true, true),
          orElse: () => expect(true, false),
        ),
        (_) => expect(true, false),
      );
    });
  });
}

void _disposeLocalAsset(String path) {
  final Directory dir = Directory(path);
  if (dir.existsSync()) {
    dir.deleteSync(recursive: true);
  }
}
