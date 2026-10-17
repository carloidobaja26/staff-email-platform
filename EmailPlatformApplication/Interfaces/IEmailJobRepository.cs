using EmailPlatform.Application.Common.Pagination;
using EmailPlatform.Application.Common.Responses;
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

    Task<IReadOnlyList<EmailJob>> GetRetryableJobsAsync(
        DateTimeOffset now,
        int batchSize = 100,
        CancellationToken cancellationToken = default);

    Task<(List<EmailJob> Items, int TotalNumber)> GetPagedAsync(
        Guid tenantId,
        int pageNumber,
        int pageSize,
        string? search);

    Task<(List<EmailJob> Items, int TotalNumber)> GetAdminPagedAsync(
        int pageNumber,
        int pageSize,
        string? search);

}