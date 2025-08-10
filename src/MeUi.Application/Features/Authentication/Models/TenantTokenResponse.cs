namespace MeUi.Application.Features.Authentication.Models;

public record TenantTokenResponse
{
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
    public TenantUserInfo User { get; init; } = new();
}

public record TenantUserInfo
{
    public Guid Id { get; init; }
    public string? Username { get; init; }
    public string? Email { get; init; }
    public string Name { get; init; } = string.Empty;
    public Guid TenantId { get; init; }
    public string TenantName { get; init; } = string.Empty;
    public bool IsTenantAdmin { get; init; }
}