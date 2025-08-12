using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantUsers.Commands.AssignRolesToTenantUser;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.TenantUsers.Roles;

public class AssignTenantUserRolesEndpoint : BaseEndpoint<AssignRolesToTenantUserCommand, IEnumerable<Guid>>, ITenantPermissionProvider
{
    public static string TenantPermission => "ASSIGN_ROLES:TENANT_USER";

    public override void ConfigureEndpoint()
    {
        Post("api/v1/tenants/{TenantId}/users/{UserId}/roles");
        Description(x => x.WithTags("Tenant User Roles").WithSummary("Assign roles to a tenant user"));
    }

    public override async Task HandleAsync(AssignRolesToTenantUserCommand req, CancellationToken ct)
    {
        IEnumerable<Guid> result = await Mediator.Send(req, ct);
        await SendSuccessAsync(result, "Roles assigned to tenant user successfully", ct);
    }
}
