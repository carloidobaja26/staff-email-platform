using Hangfire;

namespace EmailPlatform.Infrastructure.BackgroundJobs;

public static class HangfireJobScheduler
{
    public static void RegisterRecurringJobs()
    {
        RecurringJob.AddOrUpdate<HangfireEmailRetryJob>(
            "email-job-retry-worker",
            job => job.ExecuteAsync(),
            "* * * * *");
    }
}