using MediatR;

namespace MeUi.Application.Features.TenantAuthentication.Commands.TenantTokenRefresh;

public record TenantTokenRefreshCommand : IRequest<TenantTokenRefreshResponse>
{
    public string RefreshToken { get; set; } = string.Empty;
}

public record TenantTokenRefreshResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}