using EmailPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmailPlatform.Infrastructure.Persistence;

public class EmailPlatformDbContext : DbContext
{
    public EmailPlatformDbContext(
        DbContextOptions<EmailPlatformDbContext> options)
        : base(options)
    {
    }

    public DbSet<EmailApplication> EmailApplications => Set<EmailApplication>();

    public DbSet<EmailJob> EmailJobs => Set<EmailJob>();

    public DbSet<EmailDelivery> EmailDeliveries => Set<EmailDelivery>();
    public DbSet<Tenant> Tenants => Set<Tenant>();

    public DbSet<EmailWebhookEvent> EmailWebhookEvents
        => Set<EmailWebhookEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(EmailPlatformDbContext).Assembly);

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Tenant>().HasData(
            new Tenant
            {
                Id =  Guid.Parse("10000000-0000-0000-0000-000000000001"),
                Name = "Development",
                WebsiteName = "https://dev2.staffconnect-app.com",
                IsActive = true,
                CreatedAt = new DateTimeOffset(
                    2026, 1, 1, 0, 0, 0, TimeSpan.Zero)
            },
            new Tenant
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                Name = "Demo",
                WebsiteName = "https://demo.staffconnect-app.com",
                IsActive = true,
                CreatedAt = new DateTimeOffset(
                    2026, 1, 1, 0, 0, 0, TimeSpan.Zero)
            }
        );
    }

}