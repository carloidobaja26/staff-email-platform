using EmailPlatform.Application.Interfaces;
using EmailPlatform.Application.Models;
using EmailPlatform.Domain.Enums;
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
            _logger.LogWarning("MailDelivery webhook has no correlation tag.");
            return; // Added missing return guard
        }

        var emailJob = await _iEmailJobRepository.GetByCorrelationTagAsync(
            tag,
            cancellationToken);

        if (emailJob is null)
        {
            _logger.LogWarning(
                "No EmailJob found for correlation tag {CorrelationTag}",
                tag);
            return;
        }

        // Fixed structured logging syntax
        _logger.LogInformation("EmailJobId: {EmailJobId}", emailJob.Id);
        if (request.Type.Equals(
                "send",
                StringComparison.OrdinalIgnoreCase))
        {
            emailJob.Status = EmailJobStatus.Send;
            emailJob.SentAt = request.Timestamp;
            emailJob.ErrorCode = null;
            emailJob.ErrorMessage = null;
            emailJob.UpdatedAt = DateTimeOffset.UtcNow;

            await _iEmailJobRepository.UpdateAsync(
                emailJob,
                cancellationToken);

            _logger.LogInformation(
                "EmailJob {EmailJobId} marked as Accepted.",
                emailJob.Id);
        }

        if (request.Type.Equals(
                "click",
                StringComparison.OrdinalIgnoreCase))
        {
            emailJob.Status = EmailJobStatus.Click;
            emailJob.SentAt = request.Timestamp;
            emailJob.ErrorCode = null;
            emailJob.ErrorMessage = null;
            emailJob.UpdatedAt = DateTimeOffset.UtcNow;

            await _iEmailJobRepository.UpdateAsync(
                emailJob,
                cancellationToken);

            _logger.LogInformation(
                "EmailJob {EmailJobId} marked as Click.",
                emailJob.Id);
        }

        if (request.Type.Equals(
                "open",
                StringComparison.OrdinalIgnoreCase))
        {
            emailJob.Status = EmailJobStatus.Open;
            emailJob.SentAt = request.Timestamp;
            emailJob.ErrorCode = null;
            emailJob.ErrorMessage = null;
            emailJob.UpdatedAt = DateTimeOffset.UtcNow;

            await _iEmailJobRepository.UpdateAsync(
                emailJob,
                cancellationToken);

            _logger.LogInformation(
                "EmailJob {EmailJobId} marked as Open.",
                emailJob.Id);
        }

        if (request.Type.Equals(
                "bounce",
                StringComparison.OrdinalIgnoreCase))
        {
            emailJob.Status = EmailJobStatus.Bounce;
            emailJob.SentAt = request.Timestamp;
            emailJob.ErrorCode = null;
            emailJob.ErrorMessage = null;
            emailJob.UpdatedAt = DateTimeOffset.UtcNow;

            await _iEmailJobRepository.UpdateAsync(
                emailJob,
                cancellationToken);

            _logger.LogInformation(
                "EmailJob {EmailJobId} marked as Bounce.",
                emailJob.Id);
        }
        return;
    }
}