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

swaks \
  --server sakura.proxy.rlwy.net \
  --port 55008 \
  --from test@example.com \
  --to your-email@example.com \
  --header "Subject: Test Email from Swaks" \
  --body "This is a sample test message." \
  --no-auth
  //
  --auth-user "test-test" \
  --auth-password "test-test" 