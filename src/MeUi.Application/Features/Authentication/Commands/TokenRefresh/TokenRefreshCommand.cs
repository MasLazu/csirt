using MediatR;

namespace MeUi.Application.Features.Authentication.Commands.TokenRefresh;

public record TokenRefreshCommand : IRequest<TokenRefreshResponse>
{
    public string RefreshToken { get; set; } = string.Empty;
}

public record TokenRefreshResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}