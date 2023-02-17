protogen:
	protoc --dart_out=grpc:lib/api/core -Iprotos protos/*.proto

watchgen:
	dart run build_runner watch --delete-conflicting-outputs

buildgen:
	dart run build_runner build --delete-conflicting-outputs