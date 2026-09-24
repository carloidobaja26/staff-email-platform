using EmailPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmailPlatform.Infrastructure.Persistence.Configurations;

public class EmailWebhookEventConfiguration
    : IEntityTypeConfiguration<EmailWebhookEvent>
{
    public void Configure(
        EntityTypeBuilder<EmailWebhookEvent> builder)
    {
        builder.ToTable("email_webhook_events");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Provider)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.EventId)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.EventType)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => new
        {
            x.Provider,
            x.EventId
        })
        .IsUnique();
    }
}