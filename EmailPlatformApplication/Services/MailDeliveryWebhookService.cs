using EmailPlatform.Application.Interfaces;
using EmailPlatform.Application.Models;
using Microsoft.Extensions.Logging;

namespace EmailPlatform.Application.Services;

public class MailDeliveryWebhookService
    : IMailDeliveryWebhookService
{
    private readonly ILogger<MailDeliveryWebhookService> _logger;

    public MailDeliveryWebhookService(
        ILogger<MailDeliveryWebhookService> logger)
    {
        _logger = logger;
    }

    public Task ProcessAsync(
        MailDeliveryWebhookRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            """
            MailDelivery webhook received.
            Type: {Type}
            Email: {Email}
            Timestamp: {Timestamp}
            Broadcast: {Broadcast}
            FunnelMessage: {FunnelMessage}
            Tag: {Tag}
            BounceType: {BounceType}
            Code: {Code}
            """,
            request.Type,
            request.Email,
            request.Timestamp,
            request.Source?.Broadcast,
            request.Source?.FunnelMessage,
            request.Source?.Tag,
            request.BounceType,
            request.Code);

        return Task.CompletedTask;
    }
}