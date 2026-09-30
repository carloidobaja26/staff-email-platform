using EmailPlatform.Application.Interfaces;
using EmailPlatform.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace EmailPlatform.Application.Services;

public sealed class EmailRetryWorker : IEmailRetryWorker
{
    private readonly IEmailJobRepository _emailJobRepository;
    private readonly IEmailJobQueue _emailJobQueue;
    private readonly ILogger<EmailRetryWorker> _logger;

    public EmailRetryWorker(
        IEmailJobRepository emailJobRepository,
        IEmailJobQueue emailJobQueue,
        ILogger<EmailRetryWorker> logger)
    {
        _emailJobRepository = emailJobRepository;
        _emailJobQueue = emailJobQueue;
        _logger = logger;
    }

    public async Task ProcessAsync(
        CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        _logger.LogInformation(
            "EMAIL RETRY WORKER RUNNING at {Now}",
            now);
        var jobs =
            await _emailJobRepository.GetRetryableJobsAsync(
                now,
                100,
                cancellationToken);

        if (jobs.Count == 0)
        {
            _logger.LogDebug(
                "No email jobs are ready for retry.");

            return;
        }

        _logger.LogInformation(
            "Found {Count} email jobs ready for retry.",
            jobs.Count);

        foreach (var job in jobs)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var queue = GetQueue(job.Priority);

            _emailJobQueue.Enqueue(
                job.Id,
                queue);

            job.NextAttemptAt = null;

            await _emailJobRepository.UpdateAsync(
                job,
                cancellationToken);

            _logger.LogInformation(
                "EmailJob {EmailJobId} queued for retry on queue {Queue}. Attempt {Attempt}/{MaxAttempts}.",
                job.Id,
                queue,
                job.Attempts + 1,
                job.MaxAttempts);
        }
    }

    private static string GetQueue(
        EmailPriority priority)
    {
        return priority switch
        {
            EmailPriority.Critical => "critical",
            EmailPriority.High => "routine",
            EmailPriority.Normal => "routine",
            EmailPriority.Low => "bulk",
            _ => "routine"
        };
    }
}