namespace EmailPlatform.Application.Models;

public class QueueEmailRequest
{
    public Guid ApplicationId { get; init; }

    public string To { get; init; } = null!;

    public string? From { get; init; }

    public string Subject { get; init; } = null!;

    public string Template { get; init; } = null!;

    public string Payload { get; init; } = "{}";

    public int Category { get; init; }

    public int Priority { get; init; }

    public string? IdempotencyKey { get; init; }
}