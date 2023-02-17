import 'package:cqrs_mediator/cqrs_mediator.dart';
import 'package:dartz/dartz.dart';
import 'package:hermes_tests/application/use_cases/upload_test_use_case.dart';
import 'package:hermes_tests/di/config.dart';
import 'package:hermes_tests/domain/entities/test_metadata.dart';
import 'package:hermes_tests/domain/exceptions/storage_failures.dart';
import 'package:hermes_tests/domain/interfaces/i_test_repository.dart';
import 'package:mocktail/mocktail.dart';
import 'package:test/test.dart';

class MockTestRepository extends Mock implements ITestRepository {}

class FakeTestMetadata extends Fake implements TestMetadata {}

void main() {
  late final Map testConfig;
  late final MockTestRepository mockTestRepository;
  late final Mediator sut;

  group('Upload Test UseCase Unit Tests', () {
    setUpAll(() {
      testConfig = Config.fromJsonFile('config.json').test;
      mockTestRepository = MockTestRepository();

      registerFallbackValue(FakeTestMetadata());
      when(
        () => mockTestRepository.upload(any()),
      ).thenAnswer(
        (_) async => print('test uploaded'),
      );

      Mediator.instance.registerHandler(
        () => UploadTestAsyncQueryHandler(mockTestRepository),
      );

      sut = Mediator.instance;
    });

    test(
        'Given metadata for test existing on disk, '
        'When upload test use case is called, '
        'Then no storage failure is returned', () async {
      // Arrange
      final TestMetadata testMetadata = TestMetadata(
        problemId: 'marsx',
        testId: '2',
        srcTestRootFolder: testConfig['tempUnarchivedTestLocalPath'],
        destTestRootFolder: testConfig['tempTestRemotePath'],
      );

      // Act
      final Either<StorageFailure, Unit> result = await sut.run(
        UploadTestAsyncQuery(testMetadata: testMetadata),
      );

      // Assert
      expect(result.isRight(), true);
    });

    test(
        'Given metadata for test not existing on disk, '
        'When upload test use case is called, '
        'Then localtTestNotFound storage failure is returned', () async {
      // Arrange
      final TestMetadata testMetadata = TestMetadata(
        problemId: 'marsx',
        testId: '3',
        srcTestRootFolder: testConfig['tempUnarchivedTestLocalPath'],
        destTestRootFolder: testConfig['tempTestRemotePath'],
      );

      // Act
      final Either<StorageFailure, Unit> result = await sut.run(
        UploadTestAsyncQuery(testMetadata: testMetadata),
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
  });
}
