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

}