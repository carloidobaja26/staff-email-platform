using EmailPlatform.Application.Interfaces;
using EmailPlatform.Domain.Enums;

namespace EmailPlatform.Application.Services;

public class EmailDeliveryService : IEmailDeliveryService
{
    private readonly IEmailDeliveryRepository _repository;

    public EmailDeliveryService(
        IEmailDeliveryRepository repository)
    {
        _repository = repository;
    }

    public async Task MarkDeliveredAsync(
        string provider,
        string providerMessageId,
        CancellationToken cancellationToken = default)
    {
        var delivery =
            await _repository.GetByProviderMessageIdAsync(
                provider,
                providerMessageId,
                cancellationToken);

        if (delivery is null)
        {
            throw new InvalidOperationException(
                $"Delivery '{providerMessageId}' was not found.");
        }

        delivery.Status = EmailDeliveryStatus.Accepted;
        delivery.DeliveredAt = DateTimeOffset.UtcNow;

        delivery.EmailJob.Status =
            EmailJobStatus.Delivered;

        delivery.EmailJob.UpdatedAt =
            DateTimeOffset.UtcNow;

        await _repository.UpdateAsync(
            delivery,
            cancellationToken);
    }

    public async Task MarkDeferredAsync(
        string provider,
        string providerMessageId,
        string? errorCode,
        string? errorMessage,
        CancellationToken cancellationToken = default)
    {
        var delivery =
            await _repository.GetByProviderMessageIdAsync(
                provider,
                providerMessageId,
                cancellationToken);

        if (delivery is null)
        {
            throw new InvalidOperationException(
                $"Delivery '{providerMessageId}' was not found.");
        }

        delivery.Status =
            EmailDeliveryStatus.Deferred;

        delivery.ErrorCode = errorCode;
        delivery.ErrorMessage = errorMessage;

        delivery.EmailJob.Status =
            EmailJobStatus.Accepted;

        delivery.EmailJob.UpdatedAt =
            DateTimeOffset.UtcNow;

        await _repository.UpdateAsync(
            delivery,
            cancellationToken);
    }

    public async Task MarkBouncedAsync(
        string provider,
        string providerMessageId,
        string? errorCode,
        string? errorMessage,
        CancellationToken cancellationToken = default)
    {
        var delivery =
            await _repository.GetByProviderMessageIdAsync(
                provider,
                providerMessageId,
                cancellationToken);

        if (delivery is null)
        {
            throw new InvalidOperationException(
                $"Delivery '{providerMessageId}' was not found.");
        }

        delivery.Status =
            EmailDeliveryStatus.Bounced;

        delivery.BouncedAt =
            DateTimeOffset.UtcNow;

        delivery.ErrorCode = errorCode;
        delivery.ErrorMessage = errorMessage;

        delivery.EmailJob.Status =
            EmailJobStatus.Failed;

        delivery.EmailJob.UpdatedAt =
            DateTimeOffset.UtcNow;

        await _repository.UpdateAsync(
            delivery,
            cancellationToken);
    }
}