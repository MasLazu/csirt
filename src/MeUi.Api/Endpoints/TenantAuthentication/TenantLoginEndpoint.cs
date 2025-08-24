using MeUi.Api.Endpoints;
using MeUi.Api.Models;
using MeUi.Application.Features.TenantAuthentication.Commands.TenantLogin;

namespace MeUi.Api.Endpoints.TenantAuthentication;

public class TenantLoginEndpoint : BaseEndpoint<TenantLoginCommand, TenantAccessTokenResponse>
{
    public override void ConfigureEndpoint()
    {
        Post("api/v1/auth/tenant/login");
        AllowAnonymous();
        Description(x => x.WithTags("Tenant Authentication").WithSummary("Tenant user login"));
    }

    public override async Task HandleAsync(TenantLoginCommand req, CancellationToken ct)
    {
        TenantLoginResponse tokenResponse = await Mediator.Send(req, ct);

        HttpContext.Response.Cookies.Append("tenantRefreshToken", tokenResponse.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(30)
        });

        await SendSuccessAsync(new TenantAccessTokenResponse()
        {
            AccessToken = tokenResponse.AccessToken,
            ExpiresAt = tokenResponse.ExpiresAt,
            TenantId = tokenResponse.TenantId,
        }, "Tenant login successful", ct);
    }
}