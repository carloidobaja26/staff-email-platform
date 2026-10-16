using EmailPlatform.Application.Interfaces;
using EmailPlatform.Domain.Entities;
using EmailPlatform.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EmailPlatform.Infrastructure.Persistence.Repositories;

public class TenantRepository : ITenantRepository
{
    private readonly EmailPlatformDbContext _context;

    public TenantRepository(
        EmailPlatformDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        Tenant tenant,
        CancellationToken cancellationToken = default)
    {
        _context.Tenants.Add(tenant);

        await _context.SaveChangesAsync(
            cancellationToken);
    }

    public async Task UpdateAsync(
        Tenant tenant,
        CancellationToken cancellationToken = default)
    {
        // Tenant was loaded by this DbContext
        // and is already tracked.
        await _context.SaveChangesAsync(
            cancellationToken);
    }

    public async Task<Tenant?> GetByWebsiteNameAsync(
        string websiteName,
        CancellationToken cancellationToken = default)
    {
        return await _context.Tenants
            .FirstOrDefaultAsync(
                x => x.WebsiteName == websiteName,
                cancellationToken);
    }

    public async Task<(List<Tenant> Items, int TotalNumber)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? search)
    {
        var query = _context.Tenants
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            query = query.Where(c =>
                c.Name.Contains(search) ||
                c.WebsiteName.Contains(search));
        }

        var totalNumber = await query.CountAsync();

        var items = await query
            .OrderBy(c => c.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalNumber);

    }
}