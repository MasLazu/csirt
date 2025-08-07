using MediatR;
using MeUi.Application.Features.Authentication.Models;

namespace MeUi.Application.Features.Authentication.Commands.RefreshToken;

public record RefreshTokenCommand : IRequest<TokenResponse>
{
    public string RefreshToken { get; init; } = string.Empty;
}