using EmailPlatform.Application.Interfaces;
using EmailPlatform.Application.Models;
using EmailPlatform.Domain.Entities;
using EmailPlatform.Domain.Enums;

namespace EmailPlatform.Application.Services;

public class EmailService : IEmailService
{
    private readonly IEmailJobRepository _emailJobRepository;
    private readonly IEmailJobQueue _emailJobQueue;

    public EmailService(
        IEmailJobRepository emailJobRepository,
        IEmailJobQueue emailJobQueue)
    {
        _emailJobRepository = emailJobRepository;
        _emailJobQueue = emailJobQueue;
    }

    public async Task<Guid> QueueAsync(
        QueueEmailRequest request,
        CancellationToken cancellationToken = default)
    {
        var emailJob = new EmailJob
        {
            Id = Guid.NewGuid(),

            ApplicationId = request.ApplicationId,

            To = request.To,
            From = request.From,
            Subject = request.Subject,

            Template = request.Template,
            Payload = request.Payload,

            Category = (EmailCategory)request.Category,
            Priority = (EmailPriority)request.Priority,

            Status = EmailJobStatus.Pending,

            IdempotencyKey = request.IdempotencyKey,

            CreatedAt = DateTimeOffset.UtcNow
        };

        await _emailJobRepository.AddAsync(
            emailJob,
            cancellationToken);

        var queue = GetQueue(emailJob.Priority);

        _emailJobQueue.Enqueue(
            emailJob.Id,
            queue);

        return emailJob.Id;
    }

    private static string GetQueue(
        EmailPriority priority)
    {
        return priority switch
        {
            EmailPriority.Critical => "critical",
            EmailPriority.High => "routine",
            EmailPriority.Normal => "routine",
            EmailPriority.Low => "bulk",
            _ => "routine"
        };
    }
}