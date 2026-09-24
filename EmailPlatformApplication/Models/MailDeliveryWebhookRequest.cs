using System.Text.Json.Serialization;

namespace EmailPlatform.Application.Models;

public class MailDeliveryWebhookRequest
{
    [JsonPropertyName("type")]
    public string Type { get; init; } = null!;

    [JsonPropertyName("source")]
    public MailDeliveryWebhookSource? Source { get; init; }

    [JsonPropertyName("email")]
    public string? Email { get; init; }

    [JsonPropertyName("timestamp")]
    public DateTimeOffset Timestamp { get; init; }

    [JsonPropertyName("bouncetype")]
    public string? BounceType { get; init; }

    [JsonPropertyName("code")]
    public string? Code { get; init; }

    [JsonPropertyName("linkindex")]
    public int? LinkIndex { get; init; }

    [JsonPropertyName("agent")]
    public string? Agent { get; init; }

    [JsonPropertyName("ip")]
    public string? Ip { get; init; }

    [JsonPropertyName("device")]
    public string? Device { get; init; }

    [JsonPropertyName("os")]
    public string? Os { get; init; }

    [JsonPropertyName("browser")]
    public string? Browser { get; init; }

    [JsonPropertyName("country")]
    public string? Country { get; init; }

    [JsonPropertyName("country_code")]
    public string? CountryCode { get; init; }

    [JsonPropertyName("region")]
    public string? Region { get; init; }

    [JsonPropertyName("zip")]
    public string? Zip { get; init; }
}

public class MailDeliveryWebhookSource
{
    [JsonPropertyName("broadcast")]
    public string? Broadcast { get; init; }

    [JsonPropertyName("funnelmsg")]
    public string? FunnelMessage { get; init; }

    [JsonPropertyName("tag")]
    public string? Tag { get; init; }
}