import 'dart:async';

import 'package:cqrs_mediator/cqrs_mediator.dart';
import 'package:dartz/dartz.dart';
import 'package:grpc/grpc.dart';
import 'package:hermes_tests/api/core/hermes.pb.dart';
import 'package:hermes_tests/api/core/hermes.pbgrpc.dart' as hermes;
import 'package:hermes_tests/application/use_cases/download/download_test_use_case.dart';
import 'package:hermes_tests/application/use_cases/download/encode_test_use_case.dart';
import 'package:hermes_tests/application/use_cases/download/fragment_test_use_case.dart';
import 'package:hermes_tests/application/use_cases/upload/decode_test_use_case.dart';
import 'package:hermes_tests/application/use_cases/upload/defragment_test_use_case.dart';
import 'package:hermes_tests/application/use_cases/upload/upload_test_use_case.dart';
import 'package:hermes_tests/di/config/server_config.dart';
import 'package:hermes_tests/domain/entities/test_metadata.dart';
import 'package:hermes_tests/domain/exceptions/storage_failures.dart';
import 'package:logger/logger.dart';

class HermesGrpcServer extends hermes.HermesTestsServiceBase {
  late final Logger _logger;
  late final ServerConfig _config;
  late final Mediator _mediator;
  late final Server _server;

  HermesGrpcServer(
    this._config,
    this._mediator,
    this._logger,
  );

  @override
  Future<hermes.UploadResponse> uploadTest(
    ServiceCall call,
    Stream<hermes.UploadRequest> request,
  ) async {
    // extract metadata and chunk stream from request stream
    _logger.i('UploadTest method called');

    late final hermes.UploadResponse response;
    final chunkStreamController = StreamController<hermes.Chunk>();
    final requestBroadcastStream = request.asBroadcastStream();
    final firstPacket = await requestBroadcastStream.first;

    if (firstPacket.hasMetadata() == false) {
      _logger.e('Metadata not found');

      return hermes.UploadResponse()
        ..status = (hermes.StatusResponse()
          ..code = hermes.StatusCode.Failed
          ..message = 'Metadata not found');
    }

    final metadata = firstPacket.metadata;
    _logger.i('Metadata received: $metadata');

    final chunkBroadcastStream = requestBroadcastStream
        .where((packet) => packet.hasChunk())
        .map((packet) => packet.chunk)
        .asBroadcastStream();

    chunkBroadcastStream.listen(
      (chunk) => chunkStreamController.add(chunk),
      onDone: () async {
        await chunkStreamController.close();
        _logger.i('Chunk stream controller closed');
      },
      onError: (error) => chunkStreamController.addError(error),
      cancelOnError: true,
    );

    // call defragment use case
    final Either<StorageFailure, TestMetadata> defragmentResponse =
        await _mediator.run(
      DefragmentTestAsyncQuery(
        testMetadata: metadata,
        chunkStream: chunkStreamController.stream,
        destTestRootFolderForChunkedTest: _config.tempArchivedTestLocalPath,
        destTestRootFolderForArchivedTest: _config.tempUnarchivedTestLocalPath,
        maxTestSize: _config.testMaxSizeInBytes,
      ),
    );

    late final TestMetadata archivedTestMetadata;
    defragmentResponse.fold(
      (failure) {
        _logger.e('Defragment response received: $failure');
        response = hermes.UploadResponse()
          ..status = (hermes.StatusResponse()
            ..code = hermes.StatusCode.Failed
            ..message = failure.message);
      },
      (metadata) {
        _logger.i('Defragment response received: $metadata');
        archivedTestMetadata = metadata;
      },
    );

    if (defragmentResponse.isLeft()) {
      return response;
    }

    // call decode use case
    final Either<StorageFailure, TestMetadata> decodeResponse =
        await _mediator.run(
      DecodeTestAsyncQuery(
        testMetadata: archivedTestMetadata,
        destTestRootFolderForUnarchivedTest: _config.tempTestRemotePath,
      ),
    );

    late final TestMetadata unarchivedTestMetadata;
    decodeResponse.fold(
      (failure) {
        _logger.e('Decode response received: $failure');
        response = hermes.UploadResponse()
          ..status = (hermes.StatusResponse()
            ..code = hermes.StatusCode.Failed
            ..message = failure.message);
      },
      (metadata) {
        _logger.i('Decode response received: $metadata');
        unarchivedTestMetadata = metadata;
      },
    );

    if (decodeResponse.isLeft()) {
      return response;
    }

    // call upload use case
    final Either<StorageFailure, Unit> uploadResponse = await _mediator.run(
      UploadTestAsyncQuery(
        testMetadata: unarchivedTestMetadata,
      ),
    );

    response = uploadResponse.fold(
      (failure) {
        _logger.e('Upload response received: $failure');
        return hermes.UploadResponse()
          ..status = (hermes.StatusResponse()
            ..code = hermes.StatusCode.Failed
            ..message = failure.message);
      },
      (success) {
        _logger.i('Upload response received: success');
        return hermes.UploadResponse()
          ..status = (hermes.StatusResponse()
            ..code = hermes.StatusCode.Ok
            ..message = 'Test uploaded successfully');
      },
    );

    return response;
  }

  @override
  Stream<hermes.DownloadResponse> downloadTest(
    ServiceCall call,
    hermes.DownloadRequest request,
  ) async* {
    _logger.i('Download test method called');

    late final StreamController<DownloadResponse> responseStreamController =
        StreamController();

    final testMetadataForDownloadRequest = TestMetadata(
      problemId: request.problemId,
      testId: request.testId,
      srcTestRootFolder: _config.tempTestRemotePath,
      destTestRootFolder: _config.tempUnarchivedTestLocalPath,
    );

    // call download use case
    final Either<StorageFailure, TestMetadata> downloadResponse =
        await _mediator.run(
      DownloadTestAsyncQuery(
        testMetadata: testMetadataForDownloadRequest,
        destTestRootFolderForDownloadedTest: _config.tempArchivedTestLocalPath,
      ),
    );

    late final TestMetadata downloadedTestMetadata;
    downloadResponse.fold(
      (failure) {
        _logger.e('Download response received: $failure');
        responseStreamController.add(
          hermes.DownloadResponse()
            ..status = (hermes.StatusResponse()
              ..code = hermes.StatusCode.Failed
              ..message = failure.message),
        );
      },
      (metadata) {
        _logger.i('Download response received: $metadata');
        downloadedTestMetadata = metadata;
      },
    );

    if (downloadResponse.isLeft()) {
      yield* responseStreamController.stream;
    }

    // call encode use case
    final Either<StorageFailure, TestMetadata> encodeResponse =
        await _mediator.run(
      EncodeTestAsyncQuery(
        testMetadata: downloadedTestMetadata,
      ),
    );

    late final TestMetadata encodedTestMetadata;
    encodeResponse.fold(
      (failure) {
        _logger.e('Encode response received: $failure');
        responseStreamController.add(
          hermes.DownloadResponse()
            ..status = (hermes.StatusResponse()
              ..code = hermes.StatusCode.Failed
              ..message = failure.message),
        );
      },
      (metadata) {
        _logger.i('Encode response received: $metadata');
        encodedTestMetadata = metadata;
      },
    );

    if (encodeResponse.isLeft()) {
      yield* responseStreamController.stream;
    }

    // call fragment use case
    final Either<StorageFailure, Tuple2<Stream<Chunk>, int>> fragmentResponse =
        await _mediator.run(
      FragmentTestAsyncQuery(
        testMetadata: encodedTestMetadata,
      ),
    );

    fragmentResponse.fold(
      (failure) {
        _logger.e('Fragment response received: $failure');
        responseStreamController.add(
          hermes.DownloadResponse()
            ..status = (hermes.StatusResponse()
              ..code = hermes.StatusCode.Failed
              ..message = failure.message),
        );
      },
      (responseTuple) async {
        _logger.i('Fragment response received: $responseTuple');

        final Stream<Chunk> chunkStream = responseTuple.value1;
        final int testSize = responseTuple.value2;

        // add metadata
        responseStreamController.add(
          hermes.DownloadResponse()
            ..metadata = (hermes.Metadata()
              ..problemId = testMetadataForDownloadRequest.problemId
              ..testId = testMetadataForDownloadRequest.testId
              ..testSize = testSize),
        );
        _logger.i('Metadata added to response stream controller');

        // add status
        responseStreamController.add(
          hermes.DownloadResponse()
            ..status = (hermes.StatusResponse()
              ..code = hermes.StatusCode.Ok
              ..message = 'Test downloaded successfully'),
        );
        _logger.i('Status added to response stream controller');

        // add chunks
        chunkStream.listen(
          (chunk) {
            responseStreamController.add(
              hermes.DownloadResponse()
                ..chunk = (hermes.Chunk()..data = chunk.data),
            );
            _logger.i(
              'Chunk of ${chunk.data.length} bytes added to response stream controller',
            );
          },
          onDone: () async {
            await responseStreamController.close();
            _logger.i('Response stream controller closed');
          },
          onError: (error) => responseStreamController.addError(error),
          cancelOnError: true,
        );
        _logger.i('Chunks added to response stream controller');
      },
    );

    yield* responseStreamController.stream;
  }

  void initServices() {
    _server = Server([this]);
  }

  Future<void> start() async {
    initServices();
    await _server.serve(
      address: _config.host,
      port: _config.port,
    );
    _logger.i('Server started on ${_server.port}');
  }

  Future<void> close() async {
    await _server.shutdown();
    _logger.i('Server closed');
  }
}
