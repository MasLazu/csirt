using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantUsers.Commands.DemoteFromSuperAdmin;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.TenantUsers;

public class DemoteFromSuperAdminEndpoint : BaseEndpointWithoutResponse<DemoteFromSuperAdminCommand>, ITenantPermissionProvider
{
    public static string Permission => "DEMOTE_FROM_ADMIN:TENANT_USER";

    public override void ConfigureEndpoint()
    {
        Post("api/v1/tenant-users/{TenantUserId}/demote-super-admin");
        Description(x => x.WithTags("Tenant Users").WithSummary("Demote tenant user from super admin"));
        // TODO: Add authorization for super admin only
    }

    public override async Task HandleAsync(DemoteFromSuperAdminCommand req, CancellationToken ct)
    {
        await Mediator.Send(req, ct);
        await SendSuccessAsync("Tenant user demoted from super admin successfully", ct);
    }
}