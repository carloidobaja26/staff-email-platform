using EmailPlatform.Application.Common.Pagination;
using EmailPlatform.Application.Common.Responses;
using EmailPlatform.Application.Features.EmailWebhookEvents.DTOs;
using EmailPlatform.Application.Models;

namespace EmailPlatform.Application.Interfaces;

public interface IMailDeliveryWebhookService
{
    Task ProcessAsync(
        MailDeliveryWebhookRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<PagedResponse<EmailWebhookEventDto>>> GetPagedAsync(
        Guid emailJobId,
        PaginationRequest request);
}