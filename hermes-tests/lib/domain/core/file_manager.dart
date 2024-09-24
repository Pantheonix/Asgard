import 'dart:async';
import 'dart:io';

import 'package:firebase_dart/firebase_dart.dart';
import 'package:hermes_tests/api/core/hermes.pb.dart';

class FileManager {
  static bool isZipFile(String filePath) {
    final File file = File(filePath);
    final List<int> bytes = file.readAsBytesSync();
    if (bytes.isEmpty) {
      return false;
    }

    return bytes[0] == 0x50 &&
        bytes[1] == 0x4B &&
        bytes[2] == 0x03 &&
        bytes[3] == 0x04;
  }

  static void disposeLocalFile(String path) {
    final File file = File(path);
    if (file.existsSync()) {
      file.deleteSync();
    }
  }

  static Future<void> disposeRemoteAsset(
    FirebaseStorage storage,
    String path,
  ) async {
    await storage.ref(path).delete();
  }

  static void disposeLocalDirectory(String path) {
    final Directory dir = Directory(path);
    if (dir.existsSync()) {
      dir.deleteSync(recursive: true);
    }
  }

  static void disposeLocalDirectoryChildren(String path) {
    final Directory dir = Directory(path);
    if (dir.existsSync()) {
      dir.listSync().forEach((element) {
        if (element is File) {
          element.deleteSync();
        } else if (element is Directory) {
          element.deleteSync(recursive: true);
        }
      });
    }
  }

  static Stream<Chunk> readStreamOfChunksForFile(String inputPath) async* {
    final StreamController<Chunk> streamController = StreamController<Chunk>();

    final File file = File(inputPath);
    final Stream<List<int>> fileData = file.openRead();

    fileData.listen(
      (data) => streamController.add(Chunk()..data = data),
      onDone: () => streamController.close(),
      onError: (error) => streamController.addError(error),
      cancelOnError: true,
    );

    yield* streamController.stream;
  }

  static Future<void> createLocalDirectory(String path) async {
    final Directory dir = Directory(path);
    if (!dir.existsSync()) {
      await dir.create(recursive: true);
    }
  }

  static Future<bool> localDirectoryExists(String path) async {
    final Directory dir = Directory(path);
    return dir.existsSync();
  }

  static Future<void> createLocalFile(String path) async {
    final File file = File(path);
    if (!file.existsSync()) {
      await file.create(recursive: true);
    }
  }

  static Future<bool> localFileExists(String path) async {
    final File file = File(path);
    return file.existsSync();
  }
}
