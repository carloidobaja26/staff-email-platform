namespace EmailPlatform.Application.Models;

public class EmailMessage
{
    public string To { get; init; } = null!;

    public string? From { get; init; }

    public string Subject { get; init; } = null!;

    public string HtmlBody { get; init; } = null!;

    public string? TextBody { get; init; }

    public string? TransactionalTag { get; init; }
}