using EmailPlatform.Application.Interfaces;
using Hangfire;

namespace EmailPlatform.Infrastructure.BackgroundJobs;

public static class HangfireJobScheduler
{
    public static void RegisterRecurringJobs()
    {
        RecurringJob.AddOrUpdate<IEmailRetryWorker>(
            "email-job-retry-worker",
            worker => worker.ProcessAsync(
                CancellationToken.None),
            "* * * * *");
    }
}