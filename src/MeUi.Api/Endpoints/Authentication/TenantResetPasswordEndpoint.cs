using MeUi.Api.Models;
using MeUi.Application.Features.Authentication.Commands.ResetTenantUserPassword;

namespace MeUi.Api.Endpoints.Authentication;

public class TenantResetPasswordEndpoint : BaseEndpoint<ResetTenantUserPasswordCommand, bool>
{
    public override void ConfigureEndpoint()
    {
        Post("api/v1/tenant-auth/reset-password");
        AllowAnonymous(); // Note: In production, this would require proper token validation
        Description(x => x.WithTags("Tenant Auth").WithSummary("Reset tenant user password"));
    }

    public override async Task HandleAsync(ResetTenantUserPasswordCommand req, CancellationToken ct)
    {
        bool result = await Mediator.Send(req, ct);

        await SendSuccessAsync(result, "Password reset successfully", ct);
    }
}