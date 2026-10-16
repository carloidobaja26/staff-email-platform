using EmailPlatform.Application.Common.Pagination;
using EmailPlatform.Application.Common.Responses;
using EmailPlatform.Application.Features.Tenants.DTOs;
using EmailPlatform.Application.Models;

namespace EmailPlatform.Application.Interfaces;

public interface ITenantService
{

    Task<ApiResponse<PagedResponse<TenantDto>>> GetPagedAsync(
        PaginationRequest request);

    Task<ApiResponse<TenantDto>> GetByWebsiteNameAsync(string websiteName);
}