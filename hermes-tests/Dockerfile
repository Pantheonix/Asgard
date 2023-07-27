FROM dart:2.19.6 AS build
LABEL stage=builder

WORKDIR /app

# Resolve app dependencies.
COPY pubspec.* ./
RUN dart pub get

# Copy app source code and AOT compile it.
COPY . .

# Install protoc and generate gRPC code.
RUN apt-get update && apt-get install -y protobuf-compiler
RUN dart pub global activate protoc_plugin
RUN export PATH="$PATH":"$HOME/.pub-cache/bin"
RUN protoc --dart_out=grpc:lib/api/core --plugin=protoc-gen-dart=$HOME/.pub-cache/bin/protoc-gen-dart -Iprotos protos/*.proto

# Generate dart code.
RUN dart run build_runner build --delete-conflicting-outputs

# Build the app in AOT mode.
# RUN dart build 

# Ensure packages are still up-to-date if anything has changed.
RUN dart pub get --offline
RUN dart compile exe build/bin/hermes_tests.dart -o build/bin/server

# Build minimal serving image from AOT-compiled `/server` and required system
# libraries and configuration files stored in `/runtime/` from the build stage.
FROM scratch
COPY --from=build /runtime/ /
COPY --from=build /app/build/bin/server /app/bin/

# Start the server.
CMD ["/app/bin/server"]