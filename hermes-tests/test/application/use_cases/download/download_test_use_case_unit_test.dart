import 'package:cqrs_mediator/cqrs_mediator.dart';
import 'package:dartz/dartz.dart';
import 'package:hermes_tests/application/use_cases/download/download_test_use_case.dart';
import 'package:hermes_tests/di/config/config.dart';
import 'package:hermes_tests/di/config/server_config.dart';
import 'package:hermes_tests/domain/core/file_log_output.dart';
import 'package:hermes_tests/domain/entities/test_metadata.dart';
import 'package:hermes_tests/domain/exceptions/storage_failures.dart';
import 'package:hermes_tests/domain/interfaces/i_test_repository.dart';
import 'package:logger/logger.dart';
import 'package:mocktail/mocktail.dart';
import 'package:test/test.dart';

class MockTestRepository extends Mock implements ITestRepository {}

class FakeTestMetadata extends Fake implements TestMetadata {}

void main() {
  late final ServerConfig testConfig;
  late final MockTestRepository mockTestRepository;
  late final Mediator sut;

  group('Download Test UseCase Unit Tests', () {
    setUpAll(() {
      testConfig = ServerConfig.fromJson(
        Config.fromJsonFile('config.json').test,
      );
      mockTestRepository = MockTestRepository();
      final logger = Logger(
        output: FileLogOutput(
          testConfig.logOutputFilePath,
        ),
      );

      registerFallbackValue(FakeTestMetadata());

      Mediator.instance.registerHandler(
        () => DownloadTestAsyncQueryHandler(
          mockTestRepository,
          logger,
        ),
      );

      sut = Mediator.instance;
    });

    test(
        'Given metadata for requested test which already exists on disk '
        'or on remote storage, '
        'When download test use case is called, '
        'Then test metadata for the unarchived local test is returned',
        () async {
      // Arrange
      final TestMetadata requestTestMetadata = TestMetadata.testToDownload(
        problemId: 'marsx',
        testId: '2',
        fromDir: testConfig.remoteUnarchivedTestFolder,
        toDir: testConfig.tempLocalUnarchivedTestFolder,
        inputFilename: testConfig.inputFilename,
        outputFilename: testConfig.outputFilename,
      );

      when(
        () => mockTestRepository.download(requestTestMetadata),
      ).thenAnswer(
        (_) async => right(unit),
      );

      // Act
      final Either<StorageFailure, Unit> result = await sut.run(
        DownloadTestAsyncQuery(
          testMetadata: requestTestMetadata,
        ),
      );

      // Assert
      expect(result.isRight(), true);
    });

    test(
        'Given metadata for requested test which already exists on disk '
        'and the local test is invalid, '
        'When download test use case is called, '
        'Then localTestNotFound storage failure is returned', () async {
      // Arrange
      final TestMetadata requestTestMetadata = TestMetadata.testToDownload(
        problemId: 'marsx',
        testId: '6',
        fromDir: testConfig.remoteUnarchivedTestFolder,
        toDir: testConfig.tempLocalUnarchivedTestFolder,
        inputFilename: testConfig.inputFilename,
        outputFilename: testConfig.outputFilename,
      );

      when(
        () => mockTestRepository.download(any()),
      ).thenAnswer(
        (_) async => right(unit),
      );

      // Act
      final Either<StorageFailure, Unit> result = await sut.run(
        DownloadTestAsyncQuery(
          testMetadata: requestTestMetadata,
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
        'Given metadata for requested test which does not exist on disk '
        'and remote download fails, '
        'When download test use case is called, '
        'Then unexpected storage failure is returned', () async {
      // Arrange
      final TestMetadata requestTestMetadata = TestMetadata.testToDownload(
        problemId: 'marsx',
        testId: '5',
        fromDir: testConfig.remoteUnarchivedTestFolder,
        toDir: testConfig.tempLocalUnarchivedTestFolder,
        inputFilename: testConfig.inputFilename,
        outputFilename: testConfig.outputFilename,
      );

      when(
        () => mockTestRepository.download(any()),
      ).thenAnswer(
        (_) async => left(
          StorageFailure.unexpected(
            message: 'test download failed',
          ),
        ),
      );

      // Act
      final Either<StorageFailure, Unit> result = await sut.run(
        DownloadTestAsyncQuery(
          testMetadata: requestTestMetadata,
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

    test(
        'Given invalid test metadata, '
        'When download test use case is called, '
        'Then unexpected storage failure is returned', () async {
      // Arrange
      final TestMetadata requestTestMetadata = TestMetadata.testToUpload(
        problemId: 'marsx',
        testId: '5',
        fromDir: testConfig.remoteUnarchivedTestFolder,
        toDir: testConfig.tempLocalUnarchivedTestFolder,
        inputFilename: testConfig.inputFilename,
        outputFilename: testConfig.outputFilename,
      );

      when(
        () => mockTestRepository.download(any()),
      ).thenAnswer(
        (_) async => right(unit),
      );

      // Act
      final Either<StorageFailure, Unit> result = await sut.run(
        DownloadTestAsyncQuery(
          testMetadata: requestTestMetadata,
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
