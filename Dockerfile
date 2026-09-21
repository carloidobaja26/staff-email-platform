FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY EmailPlatform.sln ./
COPY EmailPlatformApi/EmailPlatformApi.csproj EmailPlatformApi/
COPY EmailPlatformApplication/EmailPlatformApplication.csproj EmailPlatformApplication/
COPY EmailPlatformDomain/EmailPlatformDomain.csproj EmailPlatformDomain/
COPY EmailPlatformInfrastructure/EmailPlatformInfrastructure.csproj EmailPlatformInfrastructure/

RUN dotnet restore EmailPlatform.sln

COPY . .

RUN dotnet publish EmailPlatformApi/EmailPlatformApi.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "EmailPlatformApi.dll"]