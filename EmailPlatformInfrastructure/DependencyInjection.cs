using EmailPlatform.Infrastructure.BackgroundJobs;
using EmailPlatform.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using EmailPlatform.Application.Interfaces;
using EmailPlatform.Infrastructure.Persistence.Repositories;
using EmailPlatform.Application.Services;
using EmailPlatform.Infrastructure.EmailProviders;

namespace EmailPlatform.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<EmailPlatformDbContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddEmailHangfire(configuration);
        services.AddScoped<IEmailJobRepository, EmailJobRepository>();
        services.AddScoped<IEmailJobProcessor, EmailJobProcessor>();
        services.AddScoped<IEmailJobQueue, HangfireEmailJobQueue>();
        services.AddScoped<IEmailProvider, FakeEmailProvider>();
        services.AddScoped<IEmailService, EmailService>();
        return services;
    }
}