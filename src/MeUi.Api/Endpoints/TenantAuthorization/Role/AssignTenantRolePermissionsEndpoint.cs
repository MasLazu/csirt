using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantAuthorization.Commands.AssignTenantRolePermissions;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.TenantAuthorization.Role;

public class AssignTenantRolePermissionsEndpoint : BaseTenantAuthorizedEndpoint<AssignTenantRolePermissionsCommand, IEnumerable<Guid>, AssignTenantRolePermissionsEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string TenantPermission => "ASSIGN:ROLE_PERMISSIONS";
    public static string Permission => "ASSIGN:TENANT_ROLE_PERMISSIONS";

    public override void ConfigureEndpoint()
    {
        Put("api/v1/tenants/{tenantId}/roles/{RoleId}/permissions");
        Description(x => x.WithTags("Tenant Role Management").WithSummary("Assign permissions to a tenant role"));
    }

    protected override async Task HandleAuthorizedAsync(AssignTenantRolePermissionsCommand req, Guid userId, CancellationToken ct)
    {
        IEnumerable<Guid> assignedPermissions = await Mediator.Send(req, ct);
        await SendSuccessAsync(assignedPermissions, "Permissions assigned to tenant role successfully", ct);
    }
}
