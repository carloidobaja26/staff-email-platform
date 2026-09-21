using EmailPlatform.Application.Models;

namespace EmailPlatform.Application.Interfaces;

public interface IEmailProvider
{
    string Name { get; }

    Task<EmailSendResult> SendAsync(
        EmailMessage message,
        CancellationToken cancellationToken = default);
}