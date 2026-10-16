using EmailPlatform.Application.Common.Pagination;
using EmailPlatform.Application.Common.Responses;
using EmailPlatform.Application.Features.Tenants.DTOs;
using EmailPlatform.Application.Interfaces;
using EmailPlatform.Application.Models;
using EmailPlatform.Domain.Entities;
using EmailPlatform.Domain.Enums;

namespace EmailPlatform.Application.Services;

public class TenantService : ITenantService
{
    private readonly ITenantRepository _tenantRepository;

    public TenantService(
        ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<ApiResponse<PagedResponse<TenantDto>>> GetPagedAsync(
        PaginationRequest request)
    {
        if (request.PageNumber < 1)
        {
            return ApiResponseFactory.Validation<PagedResponse<TenantDto>>(
                nameof(request.PageNumber),
                "Page number must be greater than 0.");
        }

        if (request.PageSize < 1)
        {
            return ApiResponseFactory.Validation<PagedResponse<TenantDto>>(
                nameof(request.PageSize),
                "Page size must be greater than 0.");
        }

        if (request.PageSize > 100)
        {
            return ApiResponseFactory.Validation<PagedResponse<TenantDto>>(
                nameof(request.PageSize),
                "Page size cannot exceed 100.");
        }

        var result = await _tenantRepository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            request.Search);

        var response = new PagedResponse<TenantDto>
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalNumber = result.TotalNumber,
            Items = result.Items.Select(MapToDto)
        };

        return ApiResponseFactory.Success(
            response,
            "Tenants retrieved successfully.");
    }

    public async Task<ApiResponse<TenantDto>> GetByWebsiteNameAsync(string websiteName)
    {
        var tenant = await _tenantRepository.GetByWebsiteNameAsync(websiteName);

        if (tenant == null)
        {
            return ApiResponseFactory.NotFound<TenantDto>(
                "Tenant not found.");
        }

        return ApiResponseFactory.Success(
            MapToDto(tenant),
            "Tenant retrieved successfully.");
    }

    private static TenantDto MapToDto(Tenant tenant)
    {
        return new TenantDto
        {
            Id = tenant.Id,
            Name = tenant.Name,
            WebsiteName = tenant.WebsiteName,
            IsActive = tenant.IsActive,
            CreatedAt = tenant.CreatedAt,
        };
    }

}