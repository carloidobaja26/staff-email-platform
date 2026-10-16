using EmailPlatform.Application.Interfaces;
using EmailPlatform.Application.Models;
using EmailPlatform.Domain.Entities;
using EmailPlatform.Domain.Enums;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace EmailPlatform.Application.Services;

public class EmailJobProcessor : IEmailJobProcessor
{
    private readonly IEmailJobRepository _emailJobRepository;
    private readonly IEmailDeliveryRepository _emailDeliveryRepository;
    private readonly IEmailProvider _emailProvider;
    private readonly ILogger<EmailJobProcessor> _logger;

    public EmailJobProcessor(
        IEmailJobRepository emailJobRepository,
        IEmailDeliveryRepository emailDeliveryRepository,
        IEmailProvider emailProvider,
        ILogger<EmailJobProcessor> logger)
    {
        _emailJobRepository = emailJobRepository;
        _emailDeliveryRepository = emailDeliveryRepository;
        _emailProvider = emailProvider;
        _logger = logger;
    }

    public async Task ProcessAsync(
        Guid emailJobId,
        CancellationToken cancellationToken = default)
    {
        var emailJob =
            await _emailJobRepository.GetByIdAsync(
                emailJobId,
                cancellationToken);

        if (emailJob is null)
        {
            throw new InvalidOperationException(
                $"Email job '{emailJobId}' was not found.");
        }

        if (emailJob.Status == EmailJobStatus.Accepted ||
            emailJob.Status == EmailJobStatus.Delivered ||
            emailJob.Status == EmailJobStatus.Cancelled)
        {
            _logger.LogInformation(
                "EmailJob {EmailJobId} already completed with status {Status}.",
                emailJob.Id,
                emailJob.Status);

            return;
        }

        // ---------------------------------------------------------
        // Mark job as Processing
        // ---------------------------------------------------------

        emailJob.Status = EmailJobStatus.Processing;
        emailJob.ProcessingStartedAt = DateTimeOffset.UtcNow;
        emailJob.Attempts++;
        emailJob.UpdatedAt = DateTimeOffset.UtcNow;

        await _emailJobRepository.UpdateAsync(
            emailJob,
            cancellationToken);

        // ---------------------------------------------------------
        // Build email message
        // ---------------------------------------------------------

        var payload =
            JsonSerializer.Deserialize<EmailJobPayload>(
                emailJob.Payload);

        if (payload is null)
        {
            throw new InvalidOperationException(
                $"EmailJob '{emailJob.Id}' has an invalid payload.");
        }

        var message = new EmailMessage
        {
            To = emailJob.To,
            From = emailJob.From,
            Subject = emailJob.Subject,

            HtmlBody = payload.HtmlBody,
            TextBody = payload.TextBody,

            TransactionalTag = emailJob.CorrelationTag,
            EmailJobId = emailJobId
        };

        var result = await _emailProvider.SendAsync(
            message,
            cancellationToken);

        if (!result.Success)
        {
            emailJob.Status = EmailJobStatus.Failed;
            emailJob.ErrorCode = result.ErrorCode;
            emailJob.ErrorMessage = result.ErrorMessage;
            emailJob.UpdatedAt = DateTimeOffset.UtcNow;

            _logger.LogWarning(
                "EmailJob {EmailJobId} failed. ErrorCode={ErrorCode}, Error={ErrorMessage}",
                emailJob.Id,
                result.ErrorCode,
                result.ErrorMessage);

            await _emailJobRepository.UpdateAsync(
                emailJob,
                cancellationToken);

            return;
        }

        if (!result.IsAccepted)
        {
            emailJob.Status = EmailJobStatus.Failed;
            emailJob.ErrorCode = result.ErrorCode;
            emailJob.ErrorMessage =
                result.ErrorMessage ??
                "Email provider did not accept the message.";

            emailJob.UpdatedAt = DateTimeOffset.UtcNow;

            await _emailJobRepository.UpdateAsync(
                emailJob,
                cancellationToken);

            return;
        }
        var delivery = new EmailDelivery
        {
            Id = Guid.NewGuid(),
            EmailJobId = emailJob.Id,
            Provider = _emailProvider.Name,
            ProviderMessageId = result.ProviderMessageId,
            Status = EmailDeliveryStatus.Accepted,
            SentAt = DateTimeOffset.UtcNow,
            CreatedAt = DateTimeOffset.UtcNow
        };

        // IMPORTANT:
        // Explicitly INSERT the new EmailDelivery.
        // Do not use emailJob.Deliveries.Add(delivery).
        await _emailDeliveryRepository.AddAsync(
            delivery,
            cancellationToken);

        emailJob.Status = EmailJobStatus.Accepted;
        emailJob.ErrorCode = null;
        emailJob.ErrorMessage = null;
        emailJob.UpdatedAt = DateTimeOffset.UtcNow;

        await _emailJobRepository.UpdateAsync(
            emailJob,
            cancellationToken);
    }

}