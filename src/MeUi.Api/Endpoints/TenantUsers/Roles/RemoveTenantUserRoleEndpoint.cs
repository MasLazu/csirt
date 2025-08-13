using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantUsers.Commands.RemoveRoleFromTenantUserV2;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.TenantUsers.Roles;

public class RemoveTenantUserRoleEndpoint : BaseEndpointWithoutResponse<RemoveRoleFromTenantUserV2Command>, ITenantPermissionProvider, IPermissionProvider
{
    public static string TenantPermission => "REMOVE_ROLE:TENANT_USER";
    public static string Permission => "REMOVE_ROLE:TENANT_USER";

    public override void ConfigureEndpoint()
    {
        Delete("api/v1/tenants/{TenantId}/users/{UserId}/roles/{RoleId}");
        Description(x => x.WithTags("Tenant User Roles").WithSummary("Remove a role from a tenant user"));
    }

    public override async Task HandleAsync(RemoveRoleFromTenantUserV2Command req, CancellationToken ct)
    {
        await Mediator.Send(req, ct);
        await SendSuccessAsync("Role removed from tenant user successfully", ct);
    }
}
