using EmailPlatform.Application.Interfaces;
using Hangfire;

namespace EmailPlatform.Infrastructure.BackgroundJobs;

public class HangfireEmailJob
{
    private readonly IEmailJobProcessor _processor;

    public HangfireEmailJob(IEmailJobProcessor processor)
    {
        _processor = processor;
    }

    [DisableConcurrentExecution(timeoutInSeconds: 300)]
    public async Task ExecuteAsync(Guid emailJobId)
    {
        await _processor.ProcessAsync(emailJobId);
    }
}