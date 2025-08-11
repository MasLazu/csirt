using MeUi.Api.Endpoints;
using MeUi.Api.Models;
using MeUi.Application.Exceptions;
using MeUi.Application.Features.Authentication.Commands.RefreshToken;
using MeUi.Application.Features.Authentication.Models;

namespace MeUi.Api.Endpoints.Authentication;

public class RefreshTokenEndpoint : BaseEndpointWithoutRequest<AccessTokenResponseData>
{
    public override void ConfigureEndpoint()
    {
        Post("api/v1/auth/refresh");
        AllowAnonymous();
        Description(x => x.WithTags("Auth").WithSummary("Refresh access token"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        string? refreshToken = HttpContext.Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(refreshToken))
        {
            throw new UnauthorizedException("No refresh token found in cookies");
        }

        var command = new RefreshTokenCommand
        {
            RefreshToken = refreshToken
        };

        TokenResponse tokenResponse = await Mediator.Send(command, ct);

        await SendSuccessAsync(new AccessTokenResponseData()
        {
            AccessToken = tokenResponse.AccessToken,
            ExpiresAt = tokenResponse.ExpiresAt
        }, "Refresh token successful", ct);
    }
}