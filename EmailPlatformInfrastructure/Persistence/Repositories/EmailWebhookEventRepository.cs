using EmailPlatform.Application.Interfaces;
using EmailPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmailPlatform.Infrastructure.Persistence.Repositories;

public class EmailWebhookEventRepository
    : IEmailWebhookEventRepository
{
    private readonly EmailPlatformDbContext _context;

    public EmailWebhookEventRepository(
        EmailPlatformDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsAsync(
        string provider,
        string eventId,
        CancellationToken cancellationToken = default)
    {
        return await _context.EmailWebhookEvents
            .AnyAsync(
                x =>
                    x.Provider == provider &&
                    x.EventId == eventId,
                cancellationToken);
    }

    public async Task AddAsync(
        EmailWebhookEvent webhookEvent,
        CancellationToken cancellationToken = default)
    {
        _context.EmailWebhookEvents.Add(webhookEvent);

        await _context.SaveChangesAsync(
            cancellationToken);
    }

    public async Task<(List<EmailWebhookEvent> Items, int TotalNumber)> GetPagedAsync(
        Guid emailJob,
        int pageNumber,
        int pageSize,
        string? search)
    {
        var query = _context.EmailWebhookEvents
            .AsNoTracking()
            .Where(c => c.EmailJobId == emailJob);;

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            query = query.Where(c =>
                c.EventType.Contains(search) ||
                c.Provider.Contains(search));
        }

        var totalNumber = await query.CountAsync();

        var items = await query
            .OrderByDescending(c => c.ReceivedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalNumber);

    }
}