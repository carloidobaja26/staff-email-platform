using EmailPlatform.Domain.Enums;

namespace EmailPlatform.Application.Features.Tenants.DTOs;

public class TenantDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string WebsiteName { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}