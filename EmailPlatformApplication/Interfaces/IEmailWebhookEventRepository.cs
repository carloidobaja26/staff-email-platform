using EmailPlatform.Domain.Entities;

namespace EmailPlatform.Application.Interfaces;

public interface IEmailWebhookEventRepository
{
    Task<bool> ExistsAsync(
        string provider,
        string eventId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        EmailWebhookEvent webhookEvent,
        CancellationToken cancellationToken = default);
}