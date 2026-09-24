namespace EmailPlatform.Domain.Entities;

public class EmailWebhookEvent
{
    public Guid Id { get; set; }

    public string Provider { get; set; } = null!;

    public string EventId { get; set; } = null!;

    public string EventType { get; set; } = null!;

    public DateTimeOffset ReceivedAt { get; set; }
        = DateTimeOffset.UtcNow;
}