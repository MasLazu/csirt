using MeUi.Api.Endpoints;
using MeUi.Api.Models;
using MeUi.Application.Features.Authentication.Commands.Login;
using MeUi.Application.Features.Authentication.Models;

namespace MeUi.Api.Endpoints.Authentication;

public class LoginEndpoint : BaseEndpoint<LoginCommand, AccessTokenResponseData>
{
    public override void ConfigureEndpoint()
    {
        Post("api/v1/auth/login");
        AllowAnonymous();
        Description(x => x.WithTags("Auth").WithSummary("User login"));
    }

    public override async Task HandleAsync(LoginCommand req, CancellationToken ct)
    {
        TokenResponse tokenResponse = await Mediator.Send(req, ct);

        HttpContext.Response.Cookies.Append("refreshToken", tokenResponse.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(30)
        });

        await SendSuccessAsync(new AccessTokenResponseData()
        {
            AccessToken = tokenResponse.AccessToken,
            ExpiresAt = tokenResponse.ExpiresAt
        }, "Login successful", ct);
    }
}