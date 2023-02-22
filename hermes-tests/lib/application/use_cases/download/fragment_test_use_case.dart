import 'dart:async';
import 'dart:io';

import 'package:cqrs_mediator/cqrs_mediator.dart';
import 'package:dartz/dartz.dart';
import 'package:hermes_tests/api/core/hermes.pb.dart';
import 'package:hermes_tests/domain/entities/test_metadata.dart';
import 'package:hermes_tests/domain/exceptions/storage_failures.dart';
import 'package:logger/logger.dart';

class FragmentTestAsyncQuery
    extends IAsyncQuery<Either<StorageFailure, Stream<Chunk>>> {
  final TestMetadata testMetadata;

  FragmentTestAsyncQuery({
    required this.testMetadata,
  });
}

class FragmentTestAsyncQueryHandler extends IAsyncQueryHandler<
    Either<StorageFailure, Stream<Chunk>>, FragmentTestAsyncQuery> {
  final Logger _logger;

  FragmentTestAsyncQueryHandler(
    this._logger,
  );

  @override
  Future<Either<StorageFailure, Stream<Chunk>>> call(
    FragmentTestAsyncQuery command,
  ) async {
    _logger.i(
      'Calling Fragment UseCase for test ${command.testMetadata.testId}...',
    );

    final StreamController<Chunk> chunkStreamController =
        StreamController<Chunk>();

    // check if archived test exists
    final File archivedTestFile = File(command.testMetadata.archivedTestPath);
    if (archivedTestFile.existsSync() == false) {
      final message =
          'Test ${command.testMetadata.testId} does not exist in ${command.testMetadata.archivedTestPath}';
      _logger.e(message);

      return left(
        StorageFailure.localTestNotFound(
          message: message,
        ),
      );
    }

    // check if archived test is a zip file
    if (_isZipFile(command.testMetadata.archivedTestPath) == false) {
      final message =
          'Test ${command.testMetadata.testId} is not a zip file in ${command.testMetadata.archivedTestPath}';
      _logger.e(message);

      return left(
        StorageFailure.invalidLocalTestFormat(
          message: message,
        ),
      );
    }

    final Stream<List<int>> archivedTestData = archivedTestFile.openRead();

    archivedTestData.listen(
      (data) => chunkStreamController.add(Chunk()..data = data),
      onDone: () => chunkStreamController.close(),
      onError: (error) => chunkStreamController.addError(error),
      cancelOnError: true,
    );

    _logger.i(
      'Fragmented test ${command.testMetadata.testId} successfully!',
    );

    return Future.value(
      right(
        chunkStreamController.stream,
      ),
    );
  }
}

bool _isZipFile(String filePath) {
  final File file = File(filePath);
  final List<int> bytes = file.readAsBytesSync();
  if (bytes.isEmpty) {
    return false;
  }

  final String fileExtension = file.path.split('.').last;

  return fileExtension == 'zip' &&
      bytes[0] == 0x50 &&
      bytes[1] == 0x4B &&
      bytes[2] == 0x03 &&
      bytes[3] == 0x04;
}
