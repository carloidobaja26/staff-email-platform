namespace EmailPlatform.Application.Interfaces;

public interface IEmailRetryWorker
{
    Task ProcessAsync(
        CancellationToken cancellationToken = default);
}
