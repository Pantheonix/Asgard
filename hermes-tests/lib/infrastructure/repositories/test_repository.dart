import 'dart:io';

import 'package:firebase_dart/firebase_dart.dart';
import 'package:hermes_tests/domain/entities/test_metadata.dart';
import 'package:hermes_tests/domain/interfaces/i_test_repository.dart';
import 'package:injectable/injectable.dart';

@LazySingleton(as: ITestRepository)
class TestRepository implements ITestRepository {
  final FirebaseStorage _storage;

  TestRepository(
    this._storage,
  );

  @override
  Future<void> upload(TestMetadata testMetadata) async {
    final File localInputFile = File(testMetadata.srcTestInputPath);
    await _storage
        .ref(testMetadata.destTestInputPath)
        .putData(localInputFile.readAsBytesSync());

    final File localOutputFile = File(testMetadata.srcTestOutputPath);
    await _storage
        .ref(testMetadata.destTestOutputPath)
        .putData(localOutputFile.readAsBytesSync());
  }

  @override
  Future<void> download(TestMetadata testMetadata) async {
    final Directory localTestRootFolder = Directory(
      '${testMetadata.destTestRootFolder}/${testMetadata.testRelativePath}',
    );
    localTestRootFolder.createSync();

    final File localInputFile = File(testMetadata.destTestInputPath);
    localInputFile.createSync();

    localInputFile.writeAsBytesSync(
      (await _storage.ref(testMetadata.srcTestInputPath).getData())
          as List<int>,
    );

    final File localOutputFile = File(testMetadata.destTestOutputPath);
    localOutputFile.createSync();

    localOutputFile.writeAsBytesSync(
      (await _storage.ref(testMetadata.srcTestOutputPath).getData())
          as List<int>,
    );
  }
}
