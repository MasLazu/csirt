namespace MeUi.Application.Features.TenantUsers.Models;

public record UpdateTenantUserRequest
{
    public Guid Id { get; init; }
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public bool IsSuspended { get; init; }
    public bool IsTenantAdmin { get; init; }
}