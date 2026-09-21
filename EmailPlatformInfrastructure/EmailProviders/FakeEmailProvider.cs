using EmailPlatform.Application.Interfaces;
using EmailPlatform.Application.Models;
using Microsoft.Extensions.Logging;

namespace EmailPlatform.Infrastructure.EmailProviders;

public class FakeEmailProvider : IEmailProvider
{
    private readonly ILogger<FakeEmailProvider> _logger;

    public FakeEmailProvider(
        ILogger<FakeEmailProvider> logger)
    {
        _logger = logger;
    }

    public string Name => "fake";

    public Task<EmailSendResult> SendAsync(
        EmailMessage message,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "FAKE EMAIL SENT. To: {To}, Subject: {Subject}",
            message.To,
            message.Subject);

        return Task.FromResult(
            new EmailSendResult
            {
                Success = true,
                ProviderMessageId = $"fake-{Guid.NewGuid():N}"
            });
    }
}