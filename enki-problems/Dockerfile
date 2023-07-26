# Restore files Stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS prepare-restore-files
LABEL stage=builder
ENV PATH="${PATH}:/root/.dotnet/tools"
RUN dotnet tool install --global --no-cache dotnet-subset --version 0.3.2
WORKDIR /src
COPY . .
RUN dotnet subset restore src/EnkiProblems.HttpApi.Host/EnkiProblems.HttpApi.Host.csproj --root-directory /src --output restore_subset/

# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
LABEL stage=builder
WORKDIR /src
COPY --from=prepare-restore-files /src/restore_subset .
RUN dotnet restore src/EnkiProblems.HttpApi.Host/EnkiProblems.HttpApi.Host.csproj --disable-parallel

COPY common.props ../
COPY NuGet.Config ../
COPY src/EnkiProblems.Application/ EnkiProblems.Application/
COPY src/EnkiProblems.Application.Contracts/ EnkiProblems.Application.Contracts/
COPY src/EnkiProblems.DbMigrator/ EnkiProblems.DbMigrator/
COPY src/EnkiProblems.Domain/ EnkiProblems.Domain/
COPY src/EnkiProblems.Domain.Shared/ EnkiProblems.Domain.Shared/
COPY src/EnkiProblems.GrpcClient/ EnkiProblems.GrpcClient/
COPY src/EnkiProblems.HttpApi/ EnkiProblems.HttpApi/
COPY src/EnkiProblems.HttpApi.Host/ EnkiProblems.HttpApi.Host/
COPY src/EnkiProblems.MongoDB/ EnkiProblems.MongoDB/

WORKDIR /src/EnkiProblems.HttpApi.Host
RUN dotnet build -c release --no-restore

# Publish Stage
FROM build AS publish
LABEL stage=builder
RUN dotnet publish -c release --no-build -o /publish

# Final Stage/Image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /publish
COPY --from=publish /publish .

ENTRYPOINT ["dotnet", "EnkiProblems.HttpApi.Host.dll", "--no-launch-profile"]