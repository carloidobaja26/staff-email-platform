namespace EmailPlatform.Application.Models;

public class MailDeliveryWebhookRequest
{
    public string EventId { get; init; } = null!;
    public string Event { get; init; } = null!;
    public DateTimeOffset Timestamp { get; init; }

    public string ProviderMessageId { get; init; } = null!;
    public string? Recipient { get; init; }

    public string? ErrorCode { get; init; }
    public string? ErrorMessage { get; init; }
}