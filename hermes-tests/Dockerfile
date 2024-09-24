FROM dart:2.19.2 AS build
LABEL stage=builder

WORKDIR /app
ENV PATH="${PATH}:${HOME}/.pub-cache/bin"

# Resolve app dependencies.
COPY pubspec.* ./
RUN dart pub get

# Copy app source code.
COPY . .

# Install protoc and generate gRPC code.
RUN apt-get update && apt-get install -y protobuf-compiler
RUN dart pub global activate protoc_plugin 20.0.1
RUN protoc --dart_out=grpc:lib/api/core --plugin=protoc-gen-dart=$HOME/.pub-cache/bin/protoc-gen-dart -Iprotos protos/*.proto

# Generate dart code.
RUN dart run build_runner build --delete-conflicting-outputs

# Ensure packages are still up-to-date if anything has changed and AOT compile.
RUN dart pub get --offline
RUN dart compile exe bin/hermes_tests.dart -o server

# Build minimal serving image from AOT-compiled `/server` and required system
# libraries and configuration files stored in `/runtime/` from the build stage.
FROM alpine:3.18
COPY --from=build /runtime/ /
COPY --from=build /app/server /app/server

# Start the server.
CMD ["/app/server"]