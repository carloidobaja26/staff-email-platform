using EmailPlatform.Application.Models;

namespace EmailPlatform.Application.Interfaces;

public interface IMailDeliveryWebhookService
{
    Task ProcessAsync(
        MailDeliveryWebhookRequest request,
        CancellationToken cancellationToken = default);
}