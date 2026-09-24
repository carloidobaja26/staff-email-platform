using EmailPlatform.Domain.Entities;

namespace EmailPlatform.Application.Interfaces;

public interface IEmailJobRepository
{
    Task<EmailJob?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        EmailJob emailJob,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        EmailJob emailJob,
        CancellationToken cancellationToken = default);

    Task<EmailJob?> GetByCorrelationTagAsync(
        string correlationTag,
        CancellationToken cancellationToken = default);
}