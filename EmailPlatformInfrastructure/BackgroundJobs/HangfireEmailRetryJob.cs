using EmailPlatform.Application.Interfaces;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace EmailPlatform.Infrastructure.BackgroundJobs;

public sealed class HangfireEmailRetryJob
{
    private readonly IEmailRetryWorker _retryWorker;
    private readonly ILogger<HangfireEmailRetryJob> _logger;

    public HangfireEmailRetryJob(
        IEmailRetryWorker retryWorker,
        ILogger<HangfireEmailRetryJob> logger)
    {
        _retryWorker = retryWorker;
        _logger = logger;
    }

    [DisableConcurrentExecution(timeoutInSeconds: 300)]
    public async Task ExecuteAsync()
    {
        _logger.LogInformation(
            "========== HANGFIRE RETRY JOB STARTED ==========");

        try
        {
            await _retryWorker.ProcessAsync();

            _logger.LogInformation(
                "========== HANGFIRE RETRY JOB FINISHED ==========");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "========== HANGFIRE RETRY JOB FAILED ==========");

            throw;
        }
    }
}