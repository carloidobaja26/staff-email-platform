using EmailPlatform.Application.Interfaces;
using EmailPlatform.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace EmailPlatform.Application.Services;

public sealed class EmailSubmissionService : IEmailSubmissionService
{
    private readonly IEmailJobRepository _emailJobRepository;
    private readonly IEmailJobQueue _emailJobQueue;
    private readonly ILogger<EmailSubmissionService> _logger;

    public EmailSubmissionService(
        IEmailJobRepository emailJobRepository,
        IEmailJobQueue emailJobQueue,
        ILogger<EmailSubmissionService> logger)
    {
        _emailJobRepository = emailJobRepository;
        _emailJobQueue = emailJobQueue;
        _logger = logger;
    }

    public async Task<EmailJob> SubmitAsync(
        EmailJob emailJob,
        CancellationToken cancellationToken = default)
    {
        await _emailJobRepository.AddAsync(
            emailJob,
            cancellationToken);

        var queue = GetQueue(emailJob.Priority);

        var hangfireJobId = _emailJobQueue.Enqueue(
            emailJob.Id,
            queue);

        _logger.LogInformation(
            """
            === EMAIL JOB ENQUEUED ===
            EmailJobId: {EmailJobId}
            HangfireJobId: {HangfireJobId}
            Queue: {Queue}
            To: {To}
            Subject: {Subject}
            ==========================
            """,
            emailJob.Id,
            hangfireJobId,
            queue,
            emailJob.To,
            emailJob.Subject);

        return emailJob;
    }

    private static string GetQueue(
        EmailPlatform.Domain.Enums.EmailPriority priority)
    {
        return priority switch
        {
            EmailPlatform.Domain.Enums.EmailPriority.Critical => "critical",
            EmailPlatform.Domain.Enums.EmailPriority.High => "routine",
            EmailPlatform.Domain.Enums.EmailPriority.Normal => "routine",
            EmailPlatform.Domain.Enums.EmailPriority.Low => "bulk",
            _ => "routine"
        };
    }
}