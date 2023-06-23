pbgen:
	protoc --dart_out=grpc:lib/api/core -Iprotos protos/*.proto
	dart run build_runner build --delete-conflicting-outputs

wgen:
	dart run build_runner watch --delete-conflicting-outputs

drun:
	dapr run --app-id hermes-tests-service --app-protocol grpc --app-port 4000 -- dart run bin/hermes_tests.dart
