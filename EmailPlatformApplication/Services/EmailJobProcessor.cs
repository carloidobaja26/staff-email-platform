using EmailPlatform.Application.Interfaces;
using EmailPlatform.Application.Models;
using EmailPlatform.Domain.Entities;
using EmailPlatform.Domain.Enums;
using Microsoft.Extensions.Logging;

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
        _logger.LogInformation(
            "Processing EmailJob {EmailJobId}",
            emailJobId);

        var emailJob =
            await _emailJobRepository.GetByIdAsync(
                emailJobId,
                cancellationToken);

        if (emailJob is null)
        {
            _logger.LogWarning(
                "EmailJob {EmailJobId} was not found.",
                emailJobId);

            throw new InvalidOperationException(
                $"Email job '{emailJobId}' was not found.");
        }

        _logger.LogInformation(
            "Loaded EmailJob {EmailJobId}. Status={Status}, Attempts={Attempts}",
            emailJob.Id,
            emailJob.Status,
            emailJob.Attempts);

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

        _logger.LogInformation(
            "Saving EmailJob {EmailJobId} as Processing.",
            emailJob.Id);

        await _emailJobRepository.UpdateAsync(
            emailJob,
            cancellationToken);

        _logger.LogInformation(
            "EmailJob {EmailJobId} marked Processing.",
            emailJob.Id);

        // ---------------------------------------------------------
        // Build email message
        // ---------------------------------------------------------

        var message = new EmailMessage
        {
            To = emailJob.To,
            From = emailJob.From,
            Subject = emailJob.Subject,
            HtmlBody = $"<p>Template: {emailJob.Template}</p>",
            TextBody = $"Template: {emailJob.Template}",
            TransactionalTag = $"email-job:{emailJob.Id}"
        };

        // ---------------------------------------------------------
        // Send email
        // ---------------------------------------------------------

        _logger.LogInformation(
            "Sending EmailJob {EmailJobId} to {Recipient} via {Provider}.",
            emailJob.Id,
            emailJob.To,
            _emailProvider.Name);

        var result = await _emailProvider.SendAsync(
            message,
            cancellationToken);

        // ---------------------------------------------------------
        // Provider failed
        // ---------------------------------------------------------

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

        // ---------------------------------------------------------
        // Provider did not accept the message
        // ---------------------------------------------------------

        if (!result.IsAccepted)
        {
            emailJob.Status = EmailJobStatus.Failed;
            emailJob.ErrorCode = result.ErrorCode;
            emailJob.ErrorMessage =
                result.ErrorMessage ??
                "Email provider did not accept the message.";

            emailJob.UpdatedAt = DateTimeOffset.UtcNow;

            _logger.LogWarning(
                "EmailJob {EmailJobId} was not accepted by provider. ErrorCode={ErrorCode}, Error={ErrorMessage}",
                emailJob.Id,
                result.ErrorCode,
                result.ErrorMessage);

            await _emailJobRepository.UpdateAsync(
                emailJob,
                cancellationToken);

            return;
        }

        // ---------------------------------------------------------
        // Provider accepted the email
        // ---------------------------------------------------------

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

        _logger.LogInformation(
            "Creating EmailDelivery for EmailJob {EmailJobId}. Provider={Provider}, ProviderMessageId={ProviderMessageId}",
            emailJob.Id,
            _emailProvider.Name,
            result.ProviderMessageId);

        // IMPORTANT:
        // Explicitly INSERT the new EmailDelivery.
        // Do not use emailJob.Deliveries.Add(delivery).
        await _emailDeliveryRepository.AddAsync(
            delivery,
            cancellationToken);

        // ---------------------------------------------------------
        // Mark EmailJob as Accepted
        // ---------------------------------------------------------

        emailJob.Status = EmailJobStatus.Accepted;
        emailJob.ErrorCode = null;
        emailJob.ErrorMessage = null;
        emailJob.UpdatedAt = DateTimeOffset.UtcNow;

        _logger.LogInformation(
            "Saving EmailJob {EmailJobId} as Accepted. ProviderMessageId={ProviderMessageId}",
            emailJob.Id,
            result.ProviderMessageId);

        await _emailJobRepository.UpdateAsync(
            emailJob,
            cancellationToken);

        _logger.LogInformation(
            "EmailJob {EmailJobId} completed successfully.",
            emailJob.Id);
    }
}