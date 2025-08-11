using MeUi.Api.Endpoints;
using MeUi.Api.Models;
using MeUi.Application.Features.Authentication.Commands.TenantRefreshToken;
using MeUi.Application.Features.Authentication.Models;

namespace MeUi.Api.Endpoints.Authentication;

public class TenantRefreshTokenEndpoint : BaseEndpoint<TenantRefreshTokenCommand, AccessTokenResponseData>
{
    public override void ConfigureEndpoint()
    {
        Post("api/v1/tenant-auth/refresh");
        AllowAnonymous();
        Description(x => x.WithTags("Tenant Auth").WithSummary("Refresh tenant access token"));
    }

    public override async Task HandleAsync(TenantRefreshTokenCommand req, CancellationToken ct)
    {
        // If no refresh token in request body, try to get it from cookie
        if (string.IsNullOrEmpty(req.RefreshToken))
        {
            string? cookieRefreshToken = HttpContext.Request.Cookies["tenantRefreshToken"];
            if (string.IsNullOrEmpty(cookieRefreshToken))
            {
                ThrowError("Refresh token is required");
            }

            req = req with { RefreshToken = cookieRefreshToken };
        }

        TenantTokenResponse tokenResponse = await Mediator.Send(req, ct);

        HttpContext.Response.Cookies.Append("tenantRefreshToken", tokenResponse.RefreshToken, new CookieOptions
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
        }, "Token refreshed successfully", ct);
    }
}