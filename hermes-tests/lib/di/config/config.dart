import 'dart:convert';
import 'dart:io';

class Config {
  late final Map<String, dynamic> _config;

  // TODO: pass config using env vars provided via dapr key vault building block
  Config.fromJsonFile(String path) {
    _config = jsonDecode(
      File(path).readAsStringSync(),
    );
  }

  get firebase => _config['firebase'];
  get dev => _config['dev'];
  get test => _config['test'];
}
