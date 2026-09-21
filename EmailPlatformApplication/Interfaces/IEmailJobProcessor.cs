namespace EmailPlatform.Application.Interfaces;

public interface IEmailJobProcessor
{
    Task ProcessAsync(
        Guid emailJobId,
        CancellationToken cancellationToken = default);
}