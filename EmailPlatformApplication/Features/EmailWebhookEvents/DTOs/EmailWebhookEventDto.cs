using EmailPlatform.Domain.Enums;

namespace EmailPlatform.Application.Features.EmailWebhookEvents.DTOs;

public class EmailWebhookEventDto
{
    public Guid Id { get; set; }

    public string Provider { get; set; } = null!;

    public string EventId { get; set; } = null!;

    public string EventType { get; set; } = null!;
    public Guid EmailJobId { get; set; }

    public DateTimeOffset ReceivedAt { get; set; }
        = DateTimeOffset.UtcNow;

}