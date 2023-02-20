import 'dart:async';

import 'package:cqrs_mediator/cqrs_mediator.dart';
import 'package:dartz/dartz.dart';
import 'package:grpc/grpc.dart';
import 'package:hermes_tests/api/core/hermes.pbgrpc.dart';
import 'package:hermes_tests/application/use_cases/decode_test_use_case.dart';
import 'package:hermes_tests/application/use_cases/defragment_test_use_case.dart';
import 'package:hermes_tests/application/use_cases/upload_test_use_case.dart';
import 'package:hermes_tests/di/config/server_config.dart';
import 'package:hermes_tests/domain/entities/test_metadata.dart';
import 'package:hermes_tests/domain/exceptions/storage_failures.dart';
import 'package:logger/logger.dart';

class HermesGrpcServer extends HermesTestsServiceBase {
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
  Future<UploadResponse> uploadTest(
    ServiceCall call,
    Stream<UploadRequest> request,
  ) async {
    // extract metadata and chunk stream from request stream
    _logger.i('UploadTest method called');

    late final UploadResponse response;
    final chunkStreamController = StreamController<Chunk>();
    final requestBroadcastStream = request.asBroadcastStream();
    final firstPacket = await requestBroadcastStream.first;

    if (firstPacket.hasMetadata() == false) {
      _logger.e('Metadata not found');

      return UploadResponse()
        ..code = UploadStatusCode.Failed
        ..message = 'Metadata not found';
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
        response = UploadResponse()
          ..code = UploadStatusCode.Failed
          ..message = failure.message;
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
        response = UploadResponse()
          ..code = UploadStatusCode.Failed
          ..message = failure.message;
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
        return UploadResponse()
          ..code = UploadStatusCode.Failed
          ..message = failure.message;
      },
      (success) {
        _logger.i('Upload response received: success');
        return UploadResponse()
          ..code = UploadStatusCode.Ok
          ..message = 'Test uploaded successfully';
      },
    );

    return response;
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
