import 'package:cqrs_mediator/cqrs_mediator.dart';
import 'package:dartz/dartz.dart';
import 'package:hermes_tests/domain/entities/test_metadata.dart';
import 'package:hermes_tests/domain/exceptions/storage_failures.dart';
import 'package:hermes_tests/domain/interfaces/i_test_repository.dart';
import 'package:logger/logger.dart';
import 'package:path/path.dart' as path;

class DeleteTestAsyncQuery extends IAsyncQuery<Either<StorageFailure, Unit>> {
  final TestMetadata testMetadata;

  DeleteTestAsyncQuery({
    required this.testMetadata,
  });
}

class DeleteTestAsyncQueryHandler extends IAsyncQueryHandler<
    Either<StorageFailure, Unit>, DeleteTestAsyncQuery> {
  final ITestRepository _testRepository;
  final Logger _logger;

  DeleteTestAsyncQueryHandler(
    this._testRepository,
    this._logger,
  );

  @override
  Future<Either<StorageFailure, Unit>> call(
    DeleteTestAsyncQuery command,
  ) async {
    // argument guard
    return command.testMetadata.maybeMap(
      testToDelete: (testMetadata) async {
        final testRelativePath = path.join(
          testMetadata.problemId,
          testMetadata.testId,
        );
        _logger.i(
          'Calling Delete UseCase for test $testRelativePath...',
        );

        final deleteResponse = await _testRepository.delete(testMetadata);

        return deleteResponse.fold(
          (failure) {
            _logger.e(
              'Failed to delete test $testRelativePath',
            );

            return left(failure);
          },
          (unit) {
            _logger.i(
              'Test $testRelativePath deleted.',
            );

            return right(unit);
          },
        );
      },
      orElse: () => left(
        StorageFailure.unexpected(
          message: 'Invalid test metadata passed to Delete UseCase.',
        ),
      ),
    );
  }
}
