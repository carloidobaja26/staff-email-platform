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
        var emailJobId = Guid.NewGuid();
        var emailJob = new EmailJob
        {
            Id = emailJobId,

            ApplicationId = request.ApplicationId,

            To = request.To,
            From = request.From,
            Subject = request.Subject,
            Template = request.Template,
            Payload = request.Payload,

            CorrelationTag = GenerateCorrelationTag(emailJobId),

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

    private static string GenerateCorrelationTag(Guid emailJobId)
    {
        const string alphabet =
            "abcdefghijklmnopqrstuvwxyz0123456789";

        var bytes = emailJobId.ToByteArray();

        var chars = new char[13];

        for (var i = 0; i < chars.Length; i++)
        {
            chars[i] =
                alphabet[bytes[i % bytes.Length] % alphabet.Length];
        }

        return "job" + new string(chars);
    }
}