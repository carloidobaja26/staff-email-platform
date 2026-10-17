using EmailPlatform.Application.Interfaces;
using EmailPlatform.Domain.Entities;
using EmailPlatform.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EmailPlatform.Infrastructure.Persistence.Repositories;

public class EmailJobRepository : IEmailJobRepository
{
    private readonly EmailPlatformDbContext _context;

    public EmailJobRepository(
        EmailPlatformDbContext context)
    {
        _context = context;
    }

    public async Task<EmailJob?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.EmailJobs
            .Include(x => x.Application)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task AddAsync(
        EmailJob emailJob,
        CancellationToken cancellationToken = default)
    {
        _context.EmailJobs.Add(emailJob);

        await _context.SaveChangesAsync(
            cancellationToken);
    }

    public async Task UpdateAsync(
        EmailJob emailJob,
        CancellationToken cancellationToken = default)
    {
        // EmailJob was loaded by this DbContext
        // and is already tracked.
        await _context.SaveChangesAsync(
            cancellationToken);
    }

    public async Task<EmailJob?> GetByCorrelationTagAsync(
        string correlationTag,
        CancellationToken cancellationToken = default)
    {
        return await _context.EmailJobs
            .FirstOrDefaultAsync(
                x => x.CorrelationTag == correlationTag,
                cancellationToken);
    }

    public async Task<IReadOnlyList<EmailJob>> GetRetryableJobsAsync(
        DateTimeOffset now,
        int batchSize = 100,
        CancellationToken cancellationToken = default)
    {
        var query = _context.EmailJobs
            .Where(x =>
                x.NextAttemptAt != null &&
                x.NextAttemptAt <= now &&
                x.Attempts < x.MaxAttempts &&
                (
                    x.Status == EmailJobStatus.Failed ||
                    x.Status == EmailJobStatus.Bounce
                ))
            .OrderBy(x => x.NextAttemptAt)
            .Take(batchSize);

        var sql = query.ToQueryString();
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<(List<EmailJob> Items, int TotalNumber)> GetPagedAsync(
        Guid tenantId,
        int pageNumber,
        int pageSize,
        string? search)
    {
        var query = _context.EmailJobs
            .AsNoTracking()
            .Include(x => x.Application)
            .Where(c => c.TenantId == tenantId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            query = query.Where(c =>
                c.From.Contains(search) ||
                c.To.Contains(search) ||
                c.Subject.Contains(search) ||
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

    public async Task<(List<EmailJob> Items, int TotalNumber)> GetAdminPagedAsync(
        int pageNumber,
        int pageSize,
        string? search)
    {
        var query = _context.EmailJobs
            .AsNoTracking()
            .Include(x => x.Application)
            .Include(x => x.Tenant)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            query = query.Where(x =>
                (x.From != null && x.From.Contains(search)) ||
                x.To.Contains(search) ||
                x.Subject.Contains(search) ||
                (x.WebsiteName != null && x.WebsiteName.Contains(search)));
        }

        var totalNumber = await query.CountAsync();

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalNumber);
    }
}