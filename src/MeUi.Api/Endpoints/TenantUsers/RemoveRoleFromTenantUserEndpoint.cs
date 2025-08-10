using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantUsers.Commands.RemoveRoleFromTenantUser;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.TenantUsers;

public class RemoveRoleFromTenantUserEndpoint : BaseEndpointWithoutResponse<RemoveRoleFromTenantUserCommand>, ITenantPermissionProvider
{
    public static string Permission => "REMOVE_ROLE:TENANT_USER";

    public override void ConfigureEndpoint()
    {
        Delete("api/v1/tenant-users/{TenantUserId}/roles/{RoleId}");
        Description(x => x.WithTags("Tenant Users").WithSummary("Remove role from tenant user"));
        // TODO: Add authorization for tenant admin or super admin only
    }

    public override async Task HandleAsync(RemoveRoleFromTenantUserCommand req, CancellationToken ct)
    {
        await Mediator.Send(req, ct);
        await SendSuccessAsync("Role removed from tenant user successfully", ct);
    }
}