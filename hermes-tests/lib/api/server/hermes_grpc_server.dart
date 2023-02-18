import 'dart:async';

import 'package:cqrs_mediator/cqrs_mediator.dart';
import 'package:dartz/dartz.dart';
import 'package:grpc/grpc.dart';
import 'package:hermes_tests/api/core/hermes.pbgrpc.dart';
import 'package:hermes_tests/application/use_cases/decode_test_use_case.dart';
import 'package:hermes_tests/application/use_cases/defragment_test_use_case.dart';
import 'package:hermes_tests/application/use_cases/upload_test_use_case.dart';
import 'package:hermes_tests/domain/entities/test_metadata.dart';
import 'package:hermes_tests/domain/exceptions/storage_failures.dart';

class HermesGrpcServer extends HermesTestsServiceBase {
  late final Map config;
  late final Mediator mediator;
  late final Server server;

  HermesGrpcServer.fromConfig(this.config, this.mediator);

  @override
  Future<UploadResponse> uploadTest(
    ServiceCall call,
    Stream<UploadRequest> request,
  ) async {
    // extract metadata and chunk stream from request stream
    print('request received');
    late final UploadResponse response;
    final chunkStreamController = StreamController<Chunk>();
    final requestBroadcastStream = request.asBroadcastStream();
    final firstPacket = await requestBroadcastStream.first;

    if (firstPacket.hasMetadata() == false) {
      return UploadResponse()
        ..code = UploadStatusCode.Failed
        ..message = 'Metadata not found';
    }

    final metadata = firstPacket.metadata;
    print('metadata received');

    final chunkBroadcastStream = requestBroadcastStream
        .where((packet) => packet.hasChunk())
        .map((packet) => packet.chunk)
        .asBroadcastStream();

    print('chunk stream received');
    chunkBroadcastStream.listen(
      (chunk) => chunkStreamController.add(chunk),
      onDone: () async {
        await chunkStreamController.close();
        print('chunk stream controller closed');
      },
      onError: (error) => chunkStreamController.addError(error),
      cancelOnError: true,
    );

    // call defragment use case
    final Either<StorageFailure, TestMetadata> defragmentResponse =
        await mediator.run(
      DefragmentTestAsyncQuery(
        testMetadata: metadata,
        chunkStream: chunkStreamController.stream,
        destTestRootFolderForChunkedTest: config['tempArchivedTestLocalPath'],
        destTestRootFolderForArchivedTest:
            config['tempUnarchivedTestLocalPath'],
        maxTestSize: config['testMaxSizeInBytes'],
      ),
    );
    print('defragment response received');

    late final TestMetadata archivedTestMetadata;
    defragmentResponse.fold(
      (failure) {
        response = UploadResponse()
          ..code = UploadStatusCode.Failed
          ..message = failure.message;
      },
      (metadata) {
        archivedTestMetadata = metadata;
      },
    );

    if (defragmentResponse.isLeft()) {
      return response;
    }

    // call decode use case
    final Either<StorageFailure, TestMetadata> decodeResponse =
        await mediator.run(
      DecodeTestAsyncQuery(
        testMetadata: archivedTestMetadata,
        destTestRootFolderForUnarchivedTest: config['tempTestRemotePath'],
      ),
    );
    print('decode response received');

    late final TestMetadata unarchivedTestMetadata;
    decodeResponse.fold(
      (failure) {
        response = UploadResponse()
          ..code = UploadStatusCode.Failed
          ..message = failure.message;
      },
      (metadata) {
        unarchivedTestMetadata = metadata;
      },
    );

    if (decodeResponse.isLeft()) {
      return response;
    }

    // call upload use case
    final Either<StorageFailure, Unit> uploadResponse = await mediator.run(
      UploadTestAsyncQuery(
        testMetadata: unarchivedTestMetadata,
      ),
    );
    print('upload response received');

    response = uploadResponse.fold(
      (failure) {
        return UploadResponse()
          ..code = UploadStatusCode.Failed
          ..message = failure.message;
      },
      (success) {
        return UploadResponse()
          ..code = UploadStatusCode.Ok
          ..message = 'Test uploaded successfully';
      },
    );

    return response;
  }

  void initServices() {
    server = Server([this]);
  }

  Future<void> start() async {
    initServices();
    await server.serve(
      address: config['host'],
      port: config['port'],
    );
  }

  Future<void> close() async {
    await server.shutdown();
  }
}
