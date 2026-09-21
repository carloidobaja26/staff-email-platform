using EmailPlatform.Application.Interfaces;

namespace EmailPlatform.Infrastructure.BackgroundJobs;

public class HangfireEmailJob
{
    private readonly IEmailJobProcessor _processor;

    public HangfireEmailJob(
        IEmailJobProcessor processor)
    {
        _processor = processor;
    }

    public async Task ExecuteAsync(Guid emailJobId)
    {
        await _processor.ProcessAsync(emailJobId);
    }
}