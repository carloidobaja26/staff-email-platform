using EmailPlatform.Domain.Enums;

namespace EmailPlatform.Application.Interfaces;

public interface IEmailDeliveryService
{
    Task MarkDeliveredAsync(
        string provider,
        string providerMessageId,
        CancellationToken cancellationToken = default);

    Task MarkDeferredAsync(
        string provider,
        string providerMessageId,
        string? errorCode,
        string? errorMessage,
        CancellationToken cancellationToken = default);

    Task MarkBouncedAsync(
        string provider,
        string providerMessageId,
        string? errorCode,
        string? errorMessage,
        CancellationToken cancellationToken = default);
}