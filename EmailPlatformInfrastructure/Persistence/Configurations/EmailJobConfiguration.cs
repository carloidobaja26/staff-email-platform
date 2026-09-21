using EmailPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmailPlatform.Infrastructure.Persistence.Configurations;

public class EmailJobConfiguration
    : IEntityTypeConfiguration<EmailJob>
{
    public void Configure(EntityTypeBuilder<EmailJob> builder)
    {
        builder.ToTable("email_jobs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.To)
            .IsRequired()
            .HasMaxLength(320);

        builder.Property(x => x.From)
            .HasMaxLength(320);

        builder.Property(x => x.Subject)
            .IsRequired()
            .HasMaxLength(998);

        builder.Property(x => x.Template)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Payload)
            .IsRequired()
            .HasColumnType("jsonb");

        builder.Property(x => x.Category)
            .IsRequired();

        builder.Property(x => x.Priority)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.ErrorCode)
            .HasMaxLength(100);

        builder.Property(x => x.ErrorMessage)
            .HasMaxLength(2000);

        builder.Property(x => x.IdempotencyKey)
            .HasMaxLength(500);

        builder.Property(x => x.Attempts)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasIndex(x => x.Status);

        builder.HasIndex(x => x.ScheduledAt);

        builder.HasIndex(x => new
        {
            x.Status,
            x.Priority,
            x.ScheduledAt
        });

        builder.HasIndex(x => new
        {
            x.ApplicationId,
            x.IdempotencyKey
        })
        .IsUnique()
        .HasFilter("\"IdempotencyKey\" IS NOT NULL");

        builder.HasOne(x => x.Application)
            .WithMany(x => x.EmailJobs)
            .HasForeignKey(x => x.ApplicationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Deliveries)
            .WithOne(x => x.EmailJob)
            .HasForeignKey(x => x.EmailJobId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}