import 'dart:async';
import 'dart:io';

import 'package:cqrs_mediator/cqrs_mediator.dart';
import 'package:dartz/dartz.dart';
import 'package:hermes_tests/api/core/hermes.pb.dart';
import 'package:hermes_tests/domain/core/file_manager.dart';
import 'package:hermes_tests/domain/entities/test_metadata.dart';
import 'package:hermes_tests/domain/exceptions/storage_failures.dart';
import 'package:logger/logger.dart';
import 'package:path/path.dart' as path;

class FragmentTestAsyncQuery
    extends IAsyncQuery<Either<StorageFailure, Tuple2<Stream<Chunk>, int>>> {
  final TestMetadata testMetadata;

  FragmentTestAsyncQuery({
    required this.testMetadata,
  });
}

class FragmentTestAsyncQueryHandler extends IAsyncQueryHandler<
    Either<StorageFailure, Tuple2<Stream<Chunk>, int>>,
    FragmentTestAsyncQuery> {
  final Logger _logger;

  FragmentTestAsyncQueryHandler(
    this._logger,
  );

  @override
  Future<Either<StorageFailure, Tuple2<Stream<Chunk>, int>>> call(
    FragmentTestAsyncQuery command,
  ) async {
    // argument guard
    return command.testMetadata.maybeMap(
      testToFragment: (testMetadata) async {
        final testRelativePath = path.join(
          testMetadata.problemId,
          testMetadata.testId,
        );
        _logger.i(
          'Calling Fragment UseCase for test $testRelativePath...',
        );

        final StreamController<Chunk> chunkStreamController =
            StreamController<Chunk>();

        // check if archived test exists
        final localArchivedTestFilePath = path.join(
          testMetadata.fromDir,
          '$testRelativePath.${testMetadata.archiveTypeExtension}',
        );
        final File localArchivedTestFile = File(localArchivedTestFilePath);

        if (localArchivedTestFile.existsSync() == false) {
          final message =
              'Test $testRelativePath does not exist in $localArchivedTestFilePath';
          _logger.e(message);

          return left(
            StorageFailure.localTestNotFound(
              message: message,
            ),
          );
        }

        _logger.i('Archived test $testRelativePath exists!');

        // check if archived test is a zip file
        if (!FileManager.isZipFile(localArchivedTestFilePath)) {
          final message =
              'Test $testRelativePath is not a zip file in $localArchivedTestFilePath';
          _logger.e(message);

          return left(
            StorageFailure.invalidLocalTestFormat(
              message: message,
            ),
          );
        }

        _logger.i('Archived test $testRelativePath is a zip file!');

        final Stream<List<int>> localArchivedTestData =
            localArchivedTestFile.openRead();
        int bytesSent = 0;

        localArchivedTestData.listen(
          (data) {
            chunkStreamController.add(Chunk()..data = data);
            bytesSent += data.length;
            _logger.i('Sent ${data.length} bytes');
          },
          onDone: () async {
            _logger.i('Sent $bytesSent bytes in total!');

            await chunkStreamController.close();
          },
          onError: (error) => chunkStreamController.addError(error),
          cancelOnError: true,
        );

        _logger.i(
          'Fragmented test $testRelativePath successfully!',
        );

        return right(
          Tuple2(
            chunkStreamController.stream,
            localArchivedTestFile.lengthSync(),
          ),
        );
      },
      orElse: () => left(
        StorageFailure.unexpected(
          message: 'Invalid test metadata passed to FragmentTestUseCase',
        ),
      ),
    );
  }
}
