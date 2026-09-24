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
}