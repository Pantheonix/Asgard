import 'dart:convert';
import 'dart:io';

import 'package:dartz/dartz.dart';
import 'package:firebase_dart/firebase_dart.dart';
import 'package:hermes_tests/domain/entities/test_metadata.dart';
import 'package:hermes_tests/domain/exceptions/storage_failures.dart';
import 'package:hermes_tests/domain/interfaces/i_test_repository.dart';
import 'package:hermes_tests/infrastructure/repositories/test_repository.dart';
import 'package:test/test.dart';

Future<void> main() async {
  FirebaseDart.setup();

  final app = await Firebase.initializeApp(
    options: FirebaseOptions.fromMap(
      jsonDecode(
        File('firebase-config.json').readAsStringSync(),
      ),
    ),
  );

  final FirebaseStorage storage = FirebaseStorage.instanceFor(app: app);
  final String testFolderLocalPath = 'temp/test';
  final String testFolderRemotePath = 'test';

  final ITestRepository sut = TestRepository(storage);

  group('Test Repository Integration Tests', () {
    test(
        'Upload(TestMetadata): '
        'Given metadata for an existing test on disk, '
        'When upload the local test to cloud storage, '
        'Then the remote test is accessible', () async {
      // Arrange
      final testMetadata = TestMetadata(
        problemId: 'marsx',
        testId: '1',
      );
      final remoteInputFilePath = 'test/marsx/1/input.txt';
      final remoteOutputFilePath = 'test/marsx/1/output.txt';

      // Act
      final Either<StorageFailure, Unit> response = await sut.upload(
        testMetadata,
        testFolderLocalPath,
        testFolderRemotePath,
      );

      // Assert
      expect(
        response.isRight(),
        true,
      );
      expect(
        await storage.ref(remoteInputFilePath).getDownloadURL(),
        isNotNull,
      );
      expect(
        await storage.ref(remoteOutputFilePath).getDownloadURL(),
        isNotNull,
      );

      await _disposeRemoteAsset(storage, remoteInputFilePath);
      await _disposeRemoteAsset(storage, remoteOutputFilePath);
    });

    test(
        'Upload(TestMetadata): '
        'Given metadata for a non-existing test on disk, '
        'When upload the local test to cloud storage, '
        'Then the remote test is not accessible', () async {
      // Arrange
      final testMetadata = TestMetadata(
        problemId: 'marsx',
        testId: '3',
      );

      // Act
      final Either<StorageFailure, Unit> response = await sut.upload(
        testMetadata,
        testFolderLocalPath,
        testFolderRemotePath,
      );

      // Assert
      expect(
        response.isLeft(),
        true,
      );
    });
  });
}

Future<void> _disposeRemoteAsset(FirebaseStorage storage, String path) async {
  await storage.ref(path).delete();
}
