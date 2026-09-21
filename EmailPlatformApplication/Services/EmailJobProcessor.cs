using System.Text.Json;
using EmailPlatform.Application.Interfaces;
using EmailPlatform.Application.Models;
using EmailPlatform.Domain.Enums;

namespace EmailPlatform.Application.Services;

public class EmailJobProcessor : IEmailJobProcessor
{
    private readonly IEmailJobRepository _emailJobRepository;
    private readonly IEmailProvider _emailProvider;

    public EmailJobProcessor(
        IEmailJobRepository emailJobRepository,
        IEmailProvider emailProvider)
    {
        _emailJobRepository = emailJobRepository;
        _emailProvider = emailProvider;
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

        if (emailJob.Status == EmailJobStatus.Sent)
        {
            return;
        }

        if (emailJob.Status == EmailJobStatus.Cancelled)
        {
            return;
        }

        emailJob.Status = EmailJobStatus.Processing;
        emailJob.ProcessingStartedAt = DateTimeOffset.UtcNow;
        emailJob.Attempts++;
        emailJob.UpdatedAt = DateTimeOffset.UtcNow;

        await _emailJobRepository.UpdateAsync(
            emailJob,
            cancellationToken);

        var message = new EmailMessage
        {
            To = emailJob.To,
            From = emailJob.From,
            Subject = emailJob.Subject,

            // Temporary body.
            // Template rendering comes later.
            HtmlBody = $"<p>Template: {emailJob.Template}</p>",
            TextBody = $"Template: {emailJob.Template}"
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

            await _emailJobRepository.UpdateAsync(
                emailJob,
                cancellationToken);

            return;
        }

        emailJob.Status = EmailJobStatus.Sent;
        emailJob.SentAt = DateTimeOffset.UtcNow;
        emailJob.UpdatedAt = DateTimeOffset.UtcNow;
        emailJob.ErrorCode = null;
        emailJob.ErrorMessage = null;

        await _emailJobRepository.UpdateAsync(
            emailJob,
            cancellationToken);
    }
}