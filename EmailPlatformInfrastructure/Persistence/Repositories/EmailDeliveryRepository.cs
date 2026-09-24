using EmailPlatform.Application.Interfaces;
using EmailPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmailPlatform.Infrastructure.Persistence.Repositories;

public class EmailDeliveryRepository : IEmailDeliveryRepository
{
    private readonly EmailPlatformDbContext _context;

    public EmailDeliveryRepository(
        EmailPlatformDbContext context)
    {
        _context = context;
    }

    public async Task<EmailDelivery?> GetByProviderMessageIdAsync(
        string provider,
        string providerMessageId,
        CancellationToken cancellationToken = default)
    {
        return await _context.EmailDeliveries
            .Include(x => x.EmailJob)
            .FirstOrDefaultAsync(
                x =>
                    x.Provider == provider &&
                    x.ProviderMessageId == providerMessageId,
                cancellationToken);
    }

    public async Task UpdateAsync(
        EmailDelivery delivery,
        CancellationToken cancellationToken = default)
    {
        _context.EmailDeliveries.Update(delivery);

        await _context.SaveChangesAsync(
            cancellationToken);
    }

    public async Task AddAsync(
        EmailDelivery delivery,
        CancellationToken cancellationToken = default)
    {
        _context.EmailDeliveries.Add(delivery);

        await _context.SaveChangesAsync(
            cancellationToken);
    }
}