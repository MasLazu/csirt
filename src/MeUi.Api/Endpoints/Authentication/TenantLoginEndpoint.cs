using MeUi.Api.Endpoints;
using MeUi.Api.Models;
using MeUi.Application.Features.Authentication.Commands.TenantLogin;
using MeUi.Application.Features.Authentication.Models;

namespace MeUi.Api.Endpoints.Authentication;

public class TenantLoginEndpoint : BaseEndpoint<TenantLoginCommand, AccessTokenResponseData>
{
    public override void ConfigureEndpoint()
    {
        Post("api/v1/tenant-auth/login");
        AllowAnonymous();
        Description(x => x.WithTags("Tenant Auth").WithSummary("Tenant user login"));
    }

    public override async Task HandleAsync(TenantLoginCommand req, CancellationToken ct)
    {
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
        }, "Tenant login successful", ct);
    }
}