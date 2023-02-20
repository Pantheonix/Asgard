import 'dart:io';

import 'package:logger/logger.dart';

class FileLogOutput extends LogOutput {
  final String _logFilePath;
  late final File _logFile;

  FileLogOutput(this._logFilePath) {
    _logFile = File(_logFilePath);
  }

  @override
  void output(OutputEvent event) {
    for (var line in event.lines) {
      _logFile.writeAsStringSync(
        '$line\n',
        mode: FileMode.append,
      );
    }
  }
}
