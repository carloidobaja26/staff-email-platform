using EmailPlatform.Application.Interfaces;
using EmailPlatform.Application.Models;
using Microsoft.Extensions.Logging;

namespace EmailPlatform.Application.Services;

public class MailDeliveryWebhookService
    : IMailDeliveryWebhookService
{
    private readonly ILogger<MailDeliveryWebhookService> _logger;
    private readonly IEmailJobRepository _iEmailJobRepository;

    public MailDeliveryWebhookService(
        ILogger<MailDeliveryWebhookService> logger,
        IEmailJobRepository iEmailJobRepository)
    {
        _logger = logger;
        _iEmailJobRepository = iEmailJobRepository;
    }

    public async Task ProcessAsync(
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
        var tag = request.Source?.Tag;

        if (string.IsNullOrWhiteSpace(tag))
        {
            _logger.LogWarning(
                "MailDelivery webhook has no correlation tag.");

        }

        var emailJob =
             await _iEmailJobRepository.GetByCorrelationTagAsync(
                tag,
                cancellationToken);

        if (emailJob is null)
        {
            _logger.LogWarning(
                "No EmailJob found for correlation tag {CorrelationTag}",
                tag);
            return;
        }
        _logger.LogWarning(
            "MailDelivery webhook has no correlation tag.");
        _logger.LogInformation("EmailJobId: ", emailJob.Id);
        return;
    }
}