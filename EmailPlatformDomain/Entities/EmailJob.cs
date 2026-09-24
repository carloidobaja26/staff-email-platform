using EmailPlatform.Domain.Enums;

namespace EmailPlatform.Domain.Entities;

public class EmailJob
{
    public Guid Id { get; set; }

    public Guid? TenantId { get; set; }

    public Guid ApplicationId { get; set; }

    public EmailApplication Application { get; set; } = null!;

    public string To { get; set; } = null!;

    public string? From { get; set; }

    public string Subject { get; set; } = null!;

    public string Template { get; set; } = null!;

    public string Payload { get; set; } = null!;

    public EmailCategory Category { get; set; }

    public EmailPriority Priority { get; set; } = EmailPriority.Normal;

    public EmailJobStatus Status { get; set; } = EmailJobStatus.Pending;

    public int Attempts { get; set; }

    public DateTimeOffset? ScheduledAt { get; set; }

    public DateTimeOffset? ProcessingStartedAt { get; set; }

    public DateTimeOffset? SentAt { get; set; }

    public string? ErrorCode { get; set; }

    public string? ErrorMessage { get; set; }

    public string? IdempotencyKey { get; set; }
    public string? CorrelationTag { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? UpdatedAt { get; set; }

    public ICollection<EmailDelivery> Deliveries { get; set; } = new List<EmailDelivery>();
}