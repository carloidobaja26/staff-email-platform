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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(EmailPlatformDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}