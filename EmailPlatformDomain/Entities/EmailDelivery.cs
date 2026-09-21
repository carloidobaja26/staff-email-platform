using EmailPlatform.Domain.Enums;

namespace EmailPlatform.Domain.Entities;

public class EmailDelivery
{
    public Guid Id { get; set; }

    public Guid EmailJobId { get; set; }

    public EmailJob EmailJob { get; set; } = null!;

    public string Provider { get; set; } = null!;

    public string? ProviderMessageId { get; set; }

    public EmailDeliveryStatus Status { get; set; }

    public string? ErrorCode { get; set; }

    public string? ErrorMessage { get; set; }

    public DateTimeOffset? SentAt { get; set; }

    public DateTimeOffset? DeliveredAt { get; set; }

    public DateTimeOffset? BouncedAt { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}