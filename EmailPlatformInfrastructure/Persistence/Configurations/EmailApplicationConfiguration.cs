using EmailPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmailPlatform.Infrastructure.Persistence.Configurations;

public class EmailApplicationConfiguration
    : IEntityTypeConfiguration<EmailApplication>
{
    public void Configure(EntityTypeBuilder<EmailApplication> builder)
    {
        builder.ToTable("email_applications");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => x.Code)
            .IsUnique();

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();
    }
}