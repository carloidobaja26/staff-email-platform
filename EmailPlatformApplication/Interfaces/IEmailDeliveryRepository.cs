using EmailPlatform.Domain.Entities;

namespace EmailPlatform.Application.Interfaces;

public interface IEmailDeliveryRepository
{
    Task<EmailDelivery?> GetByProviderMessageIdAsync(
        string provider,
        string providerMessageId,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        EmailDelivery delivery,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        EmailDelivery delivery,
        CancellationToken cancellationToken = default);
}