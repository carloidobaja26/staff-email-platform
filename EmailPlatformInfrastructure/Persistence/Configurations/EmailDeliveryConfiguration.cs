using EmailPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmailPlatform.Infrastructure.Persistence.Configurations;

public class EmailDeliveryConfiguration
    : IEntityTypeConfiguration<EmailDelivery>
{
    public void Configure(EntityTypeBuilder<EmailDelivery> builder)
    {
        builder.ToTable("email_deliveries");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Provider)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ProviderMessageId)
            .HasMaxLength(500);

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.ErrorCode)
            .HasMaxLength(100);

        builder.Property(x => x.ErrorMessage)
            .HasMaxLength(2000);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasIndex(x => x.EmailJobId);

        builder.HasIndex(x => x.ProviderMessageId);
        builder.HasIndex(x => new
        {
            x.Provider,
            x.ProviderMessageId
        });
        builder.HasOne(x => x.EmailJob)
            .WithMany(x => x.Deliveries)
            .HasForeignKey(x => x.EmailJobId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}