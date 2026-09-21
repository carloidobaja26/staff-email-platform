using EmailPlatform.Application.Models;

namespace EmailPlatform.Application.Interfaces;

public interface IEmailService
{
    Task<Guid> QueueAsync(
        QueueEmailRequest request,
        CancellationToken cancellationToken = default);
}