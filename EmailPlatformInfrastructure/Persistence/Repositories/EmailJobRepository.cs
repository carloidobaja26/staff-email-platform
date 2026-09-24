using EmailPlatform.Application.Interfaces;
using EmailPlatform.Domain.Entities;
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
}