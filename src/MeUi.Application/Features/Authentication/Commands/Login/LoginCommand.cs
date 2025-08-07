using MediatR;
using MeUi.Application.Features.Authentication.Models;

namespace MeUi.Application.Features.Authentication.Commands.Login;

public record LoginCommand : IRequest<TokenResponse>
{
    public string EmailOrUsername { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}