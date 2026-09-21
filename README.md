# staff-email-platform
staff-email-platform solution

dotnet ef migrations add InitialCreate \
    --project EmailPlatformInfrastructure \
    --startup-project EmailPlatformApi \
    --output-dir Persistence/Migrations

dotnet ef database update \
    --project EmailPlatformInfrastructure \
    --startup-project EmailPlatformApi

dotnet build

dotnet run --project EmailPlatformApi