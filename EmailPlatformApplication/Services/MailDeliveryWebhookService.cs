using EmailPlatform.Application.Interfaces;
using EmailPlatform.Application.Models;
using EmailPlatform.Domain.Entities;

namespace EmailPlatform.Application.Services;

public class MailDeliveryWebhookService
    : IMailDeliveryWebhookService
{
    private readonly IEmailDeliveryService _emailDeliveryService;
    private readonly IEmailWebhookEventRepository _webhookEventRepository;

    public MailDeliveryWebhookService(
        IEmailDeliveryService emailDeliveryService,
        IEmailWebhookEventRepository webhookEventRepository)
    {
        _emailDeliveryService = emailDeliveryService;
        _webhookEventRepository = webhookEventRepository;
    }

    public async Task ProcessAsync(
            MailDeliveryWebhookRequest request,
            CancellationToken cancellationToken = default)
    {

        var alreadyProcessed =
            await _webhookEventRepository.ExistsAsync(
                "maildelivery",
                request.EventId,
                cancellationToken);

        if (alreadyProcessed)
        {
            return;
        }
        switch (request.Event.ToLowerInvariant())
        {
            case "delivered":
            case "delivery":
                await _emailDeliveryService.MarkDeliveredAsync(
                    "maildelivery",
                    request.ProviderMessageId,
                    cancellationToken);

                break;

            case "deferred":
            case "defer":
                await _emailDeliveryService.MarkDeferredAsync(
                    "maildelivery",
                    request.ProviderMessageId,
                    request.ErrorCode,
                    request.ErrorMessage,
                    cancellationToken);

                break;

            case "bounced":
            case "bounce":
                await _emailDeliveryService.MarkBouncedAsync(
                    "maildelivery",
                    request.ProviderMessageId,
                    request.ErrorCode,
                    request.ErrorMessage,
                    cancellationToken);

                break;

            default:
                throw new InvalidOperationException(
                    $"Unsupported MailDelivery event '{request.Event}'.");
        }
        await _webhookEventRepository.AddAsync(
            new EmailWebhookEvent
            {
                Id = Guid.NewGuid(),
                Provider = "maildelivery",
                EventId = request.EventId,
                EventType = request.Event,
                ReceivedAt = DateTimeOffset.UtcNow
            },
            cancellationToken);
    }
}