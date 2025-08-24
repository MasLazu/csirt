using MediatR;

namespace MeUi.Application.Features.TenantAuthentication.Commands.TenantLogin;

public record TenantLoginCommand : IRequest<TenantLoginResponse>
{
    public string Identifier { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public Guid? TenantId { get; set; }
}

public record TenantLoginResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public Guid TenantId { get; set; }
    public DateTime ExpiresAt { get; set; }
    public TenantUserInfo User { get; set; } = new();
}

public record TenantUserInfo
{
    public Guid Id { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid TenantId { get; set; }
    public string TenantName { get; set; } = string.Empty;
    public bool IsTenantAdmin { get; set; }
}