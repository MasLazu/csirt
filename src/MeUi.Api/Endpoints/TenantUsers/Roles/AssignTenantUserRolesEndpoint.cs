using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantUsers.Commands.AssignRolesToTenantUser;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.TenantUsers.Roles;

public class AssignTenantUserRolesEndpoint : BaseTenantAuthorizedEndpoint<AssignRolesToTenantUserCommand, IEnumerable<Guid>, AssignTenantUserRolesEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string TenantPermission => "ASSIGN_ROLES:USER";
    public static string Permission => "ASSIGN_ROLES:TENANT_USER";

    public override void ConfigureEndpoint()
    {
        Post("api/v1/tenants/{TenantId}/users/{UserId}/roles");
        Description(x => x.WithTags("Tenant User Roles").WithSummary("Assign roles to a tenant user"));
    }

    protected override async Task HandleAuthorizedAsync(AssignRolesToTenantUserCommand req, Guid userId, CancellationToken ct)
    {
        IEnumerable<Guid> result = await Mediator.Send(req, ct);
        await SendSuccessAsync(result, "Roles assigned to tenant user successfully", ct);
    }
}
