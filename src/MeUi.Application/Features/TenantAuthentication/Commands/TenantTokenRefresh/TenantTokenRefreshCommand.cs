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
    public Guid TenantId { get; set; }
    public DateTime ExpiresAt { get; set; }
}