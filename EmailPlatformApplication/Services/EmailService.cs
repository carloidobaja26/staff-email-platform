using EmailPlatform.Application.Common.Pagination;
using EmailPlatform.Application.Common.Responses;
using EmailPlatform.Application.Features.Emails.DTOs;
using EmailPlatform.Application.Interfaces;
using EmailPlatform.Application.Models;
using EmailPlatform.Domain.Entities;
using EmailPlatform.Domain.Enums;

namespace EmailPlatform.Application.Services;

public class EmailService : IEmailService
{
    private readonly IEmailJobRepository _emailJobRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly IEmailJobQueue _emailJobQueue;

    public EmailService(
        IEmailJobRepository emailJobRepository,
        ITenantRepository tenantRepository,
        IEmailJobQueue emailJobQueue)
    {
        _emailJobRepository = emailJobRepository;
        _tenantRepository = tenantRepository;
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
            TenantId = request.TenantId,
            To = request.To,
            From = request.From,
            Subject = request.Subject,
            Template = request.Template,
            Payload = request.Payload,
            WebsiteName = request.WebsiteName,
            CorrelationTag = GenerateCorrelationTag(emailJobId),

            Category = (EmailCategory)request.Category,
            Priority = (EmailPriority)request.Priority,

            Status = EmailJobStatus.Pending,

            IdempotencyKey = request.IdempotencyKey,

            CreatedAt = DateTimeOffset.UtcNow
        };

        if (!string.IsNullOrWhiteSpace(request.WebsiteName))
        {
            var tenantData = await _tenantRepository
                .GetByWebsiteNameAsync(request.WebsiteName);

            Console.WriteLine($"WebsiteName: {request.WebsiteName}");

            if (tenantData != null)
            {
                Console.WriteLine($"TenantId: {tenantData.Id}");
                Console.WriteLine($"TenantName: {tenantData.Name}");

                emailJob.TenantId = tenantData.Id;
            }
            else
            {
                Console.WriteLine(
                    $"No tenant found for WebsiteName: {request.WebsiteName}");
            }
        }
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
    public async Task<ApiResponse<PagedResponse<EmailJobDto>>> GetPagedAsync(
        Guid tenantId,
        PaginationRequest request)
    {
        if (request.PageNumber < 1)
        {
            return ApiResponseFactory.Validation<PagedResponse<EmailJobDto>>(
                nameof(request.PageNumber),
                "Page number must be greater than 0.");
        }

        if (request.PageSize < 1)
        {
            return ApiResponseFactory.Validation<PagedResponse<EmailJobDto>>(
                nameof(request.PageSize),
                "Page size must be greater than 0.");
        }

        if (request.PageSize > 100)
        {
            return ApiResponseFactory.Validation<PagedResponse<EmailJobDto>>(
                nameof(request.PageSize),
                "Page size cannot exceed 100.");
        }

        var result = await _emailJobRepository.GetPagedAsync(
            tenantId,
            request.PageNumber,
            request.PageSize,
            request.Search);

        var response = new PagedResponse<EmailJobDto>
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalNumber = result.TotalNumber,
            Items = result.Items.Select(MapToDto)
        };

        return ApiResponseFactory.Success(
            response,
            "Email Job retrieved successfully.");
    }
    
    private static EmailJobDto MapToDto(EmailJob emailJob)
    {
        return new EmailJobDto
        {
            Id = emailJob.Id,
            ApplicationName = emailJob.Application.Name,
            To = emailJob.To,
            From = emailJob.From,
            Subject = emailJob.Subject,
            Category = emailJob.Category.ToString(),
            Status = emailJob.Status.ToString(),
            ProcessingStartedAt = emailJob.ProcessingStartedAt,
            SentAt = emailJob.SentAt
        };
    }

}