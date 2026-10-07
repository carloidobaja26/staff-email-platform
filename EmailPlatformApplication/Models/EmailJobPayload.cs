namespace EmailPlatform.Application.Models;

public sealed class EmailJobPayload
{
    public string? TextBody { get; set; }

    public string? HtmlBody { get; set; }
}