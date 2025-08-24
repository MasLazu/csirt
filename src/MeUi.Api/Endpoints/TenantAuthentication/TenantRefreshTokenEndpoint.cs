using MeUi.Api.Endpoints;
using MeUi.Api.Models;
using MeUi.Application.Exceptions;
using MeUi.Application.Features.TenantAuthentication.Commands.TenantTokenRefresh;

namespace MeUi.Api.Endpoints.TenantAuthentication;

public class TenantRefreshTokenEndpoint : BaseEndpointWithoutRequest<TenantAccessTokenResponse>
{
    public override void ConfigureEndpoint()
    {
        Post("api/v1/auth/tenant/refresh");
        AllowAnonymous();
        Description(x => x.WithTags("Tenant Authentication").WithSummary("Refresh tenant access token"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        string? refreshToken = HttpContext.Request.Cookies["tenantRefreshToken"];

        if (string.IsNullOrEmpty(refreshToken))
        {
            throw new UnauthorizedException("No refresh token found in cookies");
        }

        var command = new TenantTokenRefreshCommand
        {
            RefreshToken = refreshToken
        };

        TenantTokenRefreshResponse tokenResponse = await Mediator.Send(command, ct);

        HttpContext.Response.Cookies.Append("tenantRefreshToken", tokenResponse.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(30)
        });

        await SendSuccessAsync(new TenantAccessTokenResponse
        {
            AccessToken = tokenResponse.AccessToken,
            ExpiresAt = tokenResponse.ExpiresAt,
            TenantId = tokenResponse.TenantId,
        }, "Token refreshed successfully", ct);
    }
}