using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantUsers.Commands.PromoteToSuperAdmin;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.TenantUsers;

public class PromoteToSuperAdminEndpoint : BaseEndpointWithoutResponse<PromoteToSuperAdminCommand>, ITenantPermissionProvider
{
    public static string Permission => "PROMOTE_TO_ADMIN:TENANT_USER";

    public override void ConfigureEndpoint()
    {
        Post("api/v1/tenant-users/{TenantUserId}/promote-super-admin");
        Description(x => x.WithTags("Tenant Users").WithSummary("Promote tenant user to super admin"));
        // TODO: Add authorization for super admin only
    }

    public override async Task HandleAsync(PromoteToSuperAdminCommand req, CancellationToken ct)
    {
        await Mediator.Send(req, ct);
        await SendSuccessAsync("Tenant user promoted to super admin successfully", ct);
    }
}