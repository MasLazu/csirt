using MediatR;
using MeUi.Application.Features.Authentication.Models;

namespace MeUi.Application.Features.Authentication.Commands.TenantRefreshToken;

public record TenantRefreshTokenCommand : IRequest<TenantTokenResponse>
{
    public string RefreshToken { get; init; } = string.Empty;
}