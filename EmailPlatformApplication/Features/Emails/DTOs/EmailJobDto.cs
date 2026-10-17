using EmailPlatform.Domain.Enums;

namespace EmailPlatform.Application.Features.Emails.DTOs;

public class EmailJobDto
{
    public Guid Id { get; set; }

    public string ApplicationName { get; set; }

    public string To { get; set; } = null!;

    public string? From { get; set; }

    public string Subject { get; set; } = null!;

    public string Category { get; set; }

    public string Priority { get; set; }

    public string Status { get; set; }

    public DateTimeOffset? ProcessingStartedAt { get; set; }

    public DateTimeOffset? SentAt { get; set; }
    public Guid? TenantId { get; set; }

    public Guid ApplicationId { get; set; }
    public string Template { get; set; } = null!;
    public string? WebsiteName { get; set; } = null!;
    public string Payload { get; set; } = null!;
    public int Attempts { get; set; }
    public DateTimeOffset? ScheduledAt { get; set; }

    public string? ErrorCode { get; set; }

    public string? ErrorMessage { get; set; }

    public string? IdempotencyKey { get; set; }
    public string? CorrelationTag { get; set; }

    public int MaxAttempts { get; set; } = 5;

    public DateTimeOffset? NextAttemptAt { get; set; }
}