namespace EmailPlatform.Application.Models;

public class EmailSendResult
{
    public bool Success { get; init; }

    public string? ProviderMessageId { get; init; }

    public string? ErrorCode { get; init; }

    public string? ErrorMessage { get; init; }

    public bool IsTransient { get; init; }

    public bool IsAccepted { get; init; }
}