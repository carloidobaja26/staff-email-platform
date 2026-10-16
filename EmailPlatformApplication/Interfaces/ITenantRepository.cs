using EmailPlatform.Application.Common.Pagination;
using EmailPlatform.Application.Common.Responses;
using EmailPlatform.Domain.Entities;

namespace EmailPlatform.Application.Interfaces;

public interface ITenantRepository
{

    Task AddAsync(
        Tenant tenant,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Tenant tenant,
        CancellationToken cancellationToken = default);

    Task<Tenant?> GetByWebsiteNameAsync(
        string websiteName,
        CancellationToken cancellationToken = default);

    Task<(List<Tenant> Items, int TotalNumber)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? search);
}