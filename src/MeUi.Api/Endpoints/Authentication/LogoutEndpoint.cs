using MeUi.Api.Endpoints;
using MeUi.Application.Exceptions;
using MeUi.Application.Features.Authentication.Commands.Logout;

namespace MeUi.Api.Endpoints.Authentication;

public class LogoutEndpoint : BaseEndpointWithourRequestResponse
{
    public override void ConfigureEndpoint()
    {
        Post("api/v1/auth/logout");
        Description(x => x.WithTags("Authentication").WithSummary("User logout"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        string? refreshToken = HttpContext.Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(refreshToken))
        {
            throw new UnauthorizedException("No refresh token found in cookies");
        }

        var command = new LogoutCommand
        {
            RefreshToken = refreshToken
        };

        await Mediator.Send(command, ct);
        await SendSuccessAsync("Logout successful", ct);
    }
}