import 'package:cqrs_mediator/cqrs_mediator.dart';
import 'package:dartz/dartz.dart';
import 'package:hermes_tests/api/core/hermes.pb.dart';
import 'package:hermes_tests/domain/entities/test_metadata.dart';
import 'package:hermes_tests/domain/exceptions/storage_failures.dart';
import 'package:hermes_tests/domain/interfaces/i_test_repository.dart';
import 'package:logger/logger.dart';
import 'package:path/path.dart' as path;

class GetDownloadLinkForTestAsyncQuery extends IAsyncQuery<
    Either<StorageFailure, GetDownloadLinkForTestResponse>> {
  final TestMetadata testMetadata;

  GetDownloadLinkForTestAsyncQuery({
    required this.testMetadata,
  });
}

class GetDownloadLinkForTestAsyncQueryHandler extends IAsyncQueryHandler<
    Either<StorageFailure, GetDownloadLinkForTestResponse>,
    GetDownloadLinkForTestAsyncQuery> {
  final ITestRepository _testRepository;
  final Logger _logger;

  GetDownloadLinkForTestAsyncQueryHandler(
    this._testRepository,
    this._logger,
  );

  @override
  Future<Either<StorageFailure, GetDownloadLinkForTestResponse>> call(
      GetDownloadLinkForTestAsyncQuery command) async {
    // argument guard
    return command.testMetadata.maybeMap(
      testToGetDownloadLinkFor: (testMetadata) async {
        final testRelativePath = path.join(
          testMetadata.problemId,
          testMetadata.testId,
        );

        _logger.i(
          'Calling GetDownloadLinkForTest UseCase for test $testRelativePath...',
        );

        final getDownloadLinkForTestResponse =
            await _testRepository.getDownloadLinkForTest(testMetadata);

        return getDownloadLinkForTestResponse.fold(
          (failure) {
            _logger.e(
              'Failed to get download link for test $testRelativePath',
            );

            return left(failure);
          },
          (getDownloadLinkForTestResponse) {
            _logger.i(
              'Download link for test $testRelativePath received.',
            );

            return right(getDownloadLinkForTestResponse);
          },
        );
      },
      orElse: () => left(
        StorageFailure.unexpected(
          message:
              'Invalid TestMetadata passed to GetDownloadLinkForTestAsyncQueryHandler',
        ),
      ),
    );
  }
}
