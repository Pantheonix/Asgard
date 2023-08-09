# Restore files Stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS prepare-restore-files
LABEL stage=builder
ENV PATH="${PATH}:/root/.dotnet/tools"
RUN dotnet tool install --global --no-cache dotnet-subset --version 0.3.2
WORKDIR /src
COPY . .
RUN dotnet subset restore Bootstrapper/Bootstrapper.csproj --root-directory /src --output restore_subset/

# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
LABEL stage=builder
WORKDIR /src
COPY --from=prepare-restore-files /src/restore_subset .
RUN dotnet restore Bootstrapper/Bootstrapper.csproj

COPY Bootstrapper/ Bootstrapper/
COPY Api/ Api/
COPY Application/ Application/
COPY Domain/ Domain/
COPY Infrastructure/ Infrastructure/

WORKDIR /src/Bootstrapper
RUN dotnet build -c release --no-restore

# Test Stage
# FROM build AS test
# LABEL stage=builder
# WORKDIR /src

# COPY Tests.Integration/*.csproj Tests.Integration/
# RUN dotnet restore Tests.Integration/Tests.Integration.csproj

# ENTRYPOINT ["dotnet", "test", "--no-build", "--no-restore", "--verbosity=normal", "--logger:trx"]

# Publish Stage
FROM build AS publish
LABEL stage=builder
RUN dotnet publish -c release --no-build -o /publish

# Final Stage/Image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /publish
COPY --from=publish /publish .

ENTRYPOINT ["dotnet", "Bootstrapper.dll", "--no-launch-profile"]