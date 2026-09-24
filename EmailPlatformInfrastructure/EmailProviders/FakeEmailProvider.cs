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
        var providerMessageId =
            $"fake-{Guid.NewGuid():N}";

        _logger.LogInformation(
            "FAKE EMAIL ACCEPTED. To: {To}, Subject: {Subject}, ProviderMessageId: {ProviderMessageId}",
            message.To,
            message.Subject,
            providerMessageId);

        return Task.FromResult(
            new EmailSendResult
            {
                Success = true,
                IsAccepted = true,
                ProviderMessageId = providerMessageId
            });
    }
}