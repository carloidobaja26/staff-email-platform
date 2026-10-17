using EmailPlatform.Application.Common.Pagination;
using EmailPlatform.Application.Common.Responses;
using EmailPlatform.Application.Features.Emails.DTOs;
using EmailPlatform.Application.Models;

namespace EmailPlatform.Application.Interfaces;

public interface IEmailService
{
    Task<Guid> QueueAsync(
        QueueEmailRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<PagedResponse<EmailJobDto>>> GetPagedAsync(
        Guid tenantId,
        PaginationRequest request);

    Task<ApiResponse<PagedResponse<EmailJobDto>>> GetAdminPagedAsync(
        PaginationRequest request);
    Task<ApiResponse<EmailJobDto>> GetByIdAsync(Guid id);
}