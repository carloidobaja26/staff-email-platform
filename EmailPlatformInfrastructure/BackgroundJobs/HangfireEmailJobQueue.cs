using EmailPlatform.Application.Interfaces;
using Hangfire;
using Hangfire.States;

namespace EmailPlatform.Infrastructure.BackgroundJobs;

public class HangfireEmailJobQueue : IEmailJobQueue
{
    private readonly IBackgroundJobClient _backgroundJobClient;

    public HangfireEmailJobQueue(
        IBackgroundJobClient backgroundJobClient)
    {
        _backgroundJobClient = backgroundJobClient;
    }

    public string Enqueue(
        Guid emailJobId,
        string queue)
    {
        var state = new EnqueuedState(queue);

        return _backgroundJobClient.Create<HangfireEmailJob>(
            job => job.ExecuteAsync(emailJobId),
            state);
    }
}