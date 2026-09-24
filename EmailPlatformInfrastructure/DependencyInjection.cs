using EmailPlatform.Application.Interfaces;
using EmailPlatform.Application.Services;
using EmailPlatform.Infrastructure.BackgroundJobs;
using EmailPlatform.Infrastructure.EmailProviders;
using EmailPlatform.Infrastructure.Persistence;
using EmailPlatform.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

            options.EnableDetailedErrors();
            options.EnableSensitiveDataLogging();
        });

        services.AddEmailHangfire(configuration);

        services.AddScoped<
            IEmailJobRepository,
            EmailJobRepository>();

        services.AddScoped<
            IEmailJobProcessor,
            EmailJobProcessor>();

        services.AddScoped<
            IEmailJobQueue,
            HangfireEmailJobQueue>();

        services.Configure<EmailDeliveryOptions>(
            configuration.GetSection("EmailDelivery"));

        services.AddScoped<
            IEmailProvider,
            EmailDeliveryProvider>();

        services.AddScoped<
            IEmailService,
            EmailService>();

        services.AddScoped<
            IEmailDeliveryRepository,
            EmailDeliveryRepository>();

        services.AddScoped<
            IEmailDeliveryService,
            EmailDeliveryService>();

        services.AddScoped<
            IMailDeliveryWebhookService,
            MailDeliveryWebhookService>();

        services.AddScoped<
            IEmailWebhookEventRepository,
            EmailWebhookEventRepository>();

        return services;
    }
}