using MeUi.Api.Models;
using MeUi.Application.Features.Authentication.Commands.RegisterTenantUser;
using MeUi.Application.Features.Authentication.Models;

namespace MeUi.Api.Endpoints.Authentication;

public class TenantRegisterEndpoint : BaseEndpoint<RegisterTenantUserCommand, TenantUserInfo>
{
    public override void ConfigureEndpoint()
    {
        Post("api/v1/tenant-auth/register");
        AllowAnonymous(); // Note: In production, this might need to be restricted to tenant admins
        Description(x => x.WithTags("Tenant Auth").WithSummary("Register new tenant user"));
    }

    public override async Task HandleAsync(RegisterTenantUserCommand req, CancellationToken ct)
    {
        TenantUserInfo userInfo = await Mediator.Send(req, ct);

        await SendSuccessAsync(userInfo, "Tenant user registered successfully", ct);
    }
}