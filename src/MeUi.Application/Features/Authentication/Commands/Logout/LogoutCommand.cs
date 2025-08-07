using MediatR;

namespace MeUi.Application.Features.Authentication.Commands.Logout;

public record LogoutCommand : IRequest<bool>
{
    public string RefreshToken { get; init; } = string.Empty;
}