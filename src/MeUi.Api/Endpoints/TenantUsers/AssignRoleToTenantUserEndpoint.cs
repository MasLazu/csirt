using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantUsers.Commands.AssignRoleToTenantUser;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.TenantUsers;

public class AssignRoleToTenantUserEndpoint : BaseEndpointWithoutResponse<AssignRoleToTenantUserCommand>, ITenantPermissionProvider
{
    public static string Permission => "ASSIGN_ROLE:TENANT_USER";

    public override void ConfigureEndpoint()
    {
        Post("api/v1/tenant-users/{TenantUserId}/roles/{RoleId}");
        Description(x => x.WithTags("Tenant Users").WithSummary("Assign role to tenant user"));
        // TODO: Add authorization for tenant admin or super admin only
    }

    public override async Task HandleAsync(AssignRoleToTenantUserCommand req, CancellationToken ct)
    {
        await Mediator.Send(req, ct);
        await SendSuccessAsync("Role assigned to tenant user successfully", ct);
    }
}