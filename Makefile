qdrun:
	cd quetzalcoatl-auth && dapr run --app-id quetzalcoatl-auth --app-port 5210 --dapr-http-port 35000 --components-path ../dapr/components --config ../dapr/config/config.yaml -- dotnet run --project Bootstrapper/Bootstrapper.csproj

edrun:
	cd enki-problems && dapr run --app-id enki-problems --app-port 5211 --dapr-http-port 35001 --dapr-grpc-port 50001 --components-path ../dapr/components --config ../dapr/config/config.yaml -- dotnet run --project src/EnkiProblems.HttpApi.Host/EnkiProblems.HttpApi.Host.csproj

hdrun:
	cd hermes-tests && export $$(cat .env | xargs) && echo $$HERMES_CONFIG && dapr run --app-id hermes-tests --app-port 5212 --dapr-grpc-port 50002 --config ../dapr/config/config.yaml -- dart run bin/hermes_tests.dart

adrun:
	cd anubis-eval && dapr run --app-id anubis-eval --app-port 5213 --app-protocol http --dapr-http-port 35003 --components-path ../dapr/components --config ../dapr/config/config.yaml -- cargo run

tdrun:
	cd thor-submissions && dapr run --app-id thor-submissions --app-port 5214 --app-protocol http --dapr-http-port 35004 --components-path ../dapr/components --config ../dapr/config/config.yaml -- go run main.go