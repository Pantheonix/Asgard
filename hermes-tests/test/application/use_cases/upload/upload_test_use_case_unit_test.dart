import 'package:cqrs_mediator/cqrs_mediator.dart';
import 'package:dartz/dartz.dart';
import 'package:hermes_tests/application/use_cases/upload/upload_test_use_case.dart';
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

  group('Upload Test UseCase Unit Tests', () {
    setUpAll(() {
      testConfig = ServerConfig.fromJson(
        Config.fromEnv('HERMES_CONFIG').test,
      );
      mockTestRepository = MockTestRepository();
      final logger = Logger(
        output: FileLogOutput(
          testConfig.logOutputFilePath,
        ),
      );

      registerFallbackValue(FakeTestMetadata());

      Mediator.instance.registerHandler(
        () => UploadTestAsyncQueryHandler(
          mockTestRepository,
          logger,
        ),
      );

      sut = Mediator.instance;
    });

    test(
        'Given metadata for test existing on disk, '
        'When upload test use case is called, '
        'Then no storage failure is returned', () async {
      // Arrange
      final TestMetadata testMetadata = TestMetadata.testToUpload(
        problemId: 'sum',
        testId: '2',
        fromDir: testConfig.tempLocalUnarchivedTestFolder,
        toDir: testConfig.tempLocalUnarchivedTestFolder,
        inputFilename: testConfig.inputFilename,
        outputFilename: testConfig.outputFilename,
      );

      when(
        () => mockTestRepository.upload(any()),
      ).thenAnswer(
        (_) async => right(unit),
      );

      // Act
      final Either<StorageFailure, Unit> result = await sut.run(
        UploadTestAsyncQuery(
          testMetadata: testMetadata,
        ),
      );

      // Assert
      expect(result.isRight(), true);

      verify(
        () => mockTestRepository.upload(any()),
      ).called(1);
    });

    test(
        'Given metadata for test not existing on disk, '
        'When upload test use case is called, '
        'Then localtTestNotFound storage failure is returned', () async {
      // Arrange
      final TestMetadata testMetadata = TestMetadata.testToUpload(
        problemId: 'sum',
        testId: '3',
        fromDir: testConfig.tempLocalUnarchivedTestFolder,
        toDir: testConfig.tempLocalUnarchivedTestFolder,
        inputFilename: testConfig.inputFilename,
        outputFilename: testConfig.outputFilename,
      );

      when(
        () => mockTestRepository.upload(any()),
      ).thenAnswer(
        (_) async => left(
          StorageFailure.unexpected(
            message: 'test upload failed',
          ),
        ),
      );

      // Act
      final Either<StorageFailure, Unit> result = await sut.run(
        UploadTestAsyncQuery(
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
        'Given metadata for test existing on disk and remote upload fails, '
        'When upload test use case is called, '
        'Then unexpected storage failure is returned', () async {
      // Arrange
      final TestMetadata testMetadata = TestMetadata.testToUpload(
        problemId: 'sum',
        testId: '2',
        fromDir: testConfig.tempLocalUnarchivedTestFolder,
        toDir: testConfig.tempLocalUnarchivedTestFolder,
        inputFilename: testConfig.inputFilename,
        outputFilename: testConfig.outputFilename,
      );

      when(
        () => mockTestRepository.upload(any()),
      ).thenAnswer(
        (_) async => left(
          StorageFailure.unexpected(
            message: 'test upload failed',
          ),
        ),
      );

      // Act
      final Either<StorageFailure, Unit> result = await sut.run(
        UploadTestAsyncQuery(
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

    test(
        'Given invalid test metadata, '
        'When upload test use case is called, '
        'Then unexpected storage failure is returned', () async {
      // Arrange
      final TestMetadata testMetadata = TestMetadata.testToDownload(
        problemId: 'sum',
        testId: '2',
        fromDir: testConfig.tempLocalUnarchivedTestFolder,
        toDir: testConfig.tempLocalUnarchivedTestFolder,
        inputFilename: testConfig.inputFilename,
        outputFilename: testConfig.outputFilename,
      );

      when(
        () => mockTestRepository.upload(any()),
      ).thenAnswer(
        (_) async => left(
          StorageFailure.unexpected(
            message: 'test upload failed',
          ),
        ),
      );

      // Act
      final Either<StorageFailure, Unit> result = await sut.run(
        UploadTestAsyncQuery(
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
