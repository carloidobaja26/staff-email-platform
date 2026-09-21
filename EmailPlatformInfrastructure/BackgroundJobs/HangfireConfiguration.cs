using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmailPlatform.Infrastructure.BackgroundJobs;

public static class HangfireConfiguration
{
    public static IServiceCollection AddEmailHangfire(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "DefaultConnection is not configured.");

        services.AddHangfire(config =>
        {
            config.UseSimpleAssemblyNameTypeSerializer();
            config.UseRecommendedSerializerSettings();

            config.UsePostgreSqlStorage(options =>
            {
                options.UseNpgsqlConnection(connectionString);
            });
        });

        services.AddHangfireServer(options =>
        {
            options.Queues = new[]
            {
                "critical",
                "routine",
                "bulk"
            };

            options.WorkerCount = 5;
        });

        return services;
    }
}