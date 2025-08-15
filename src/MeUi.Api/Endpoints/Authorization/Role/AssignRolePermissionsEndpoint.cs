using MeUi.Api.Endpoints;
using MeUi.Application.Features.Authorization.Commands.AssignRolePermissions;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.Authorization.Role;

public class AssignRolePermissionsEndpoint : BaseAuthorizedEndpoint<AssignRolePermissionsCommand, IEnumerable<Guid>, AssignRolePermissionsEndpoint>, IPermissionProvider
{
    public static string Permission => "ASSIGN:ROLE_PERMISSIONS";

    public override void ConfigureEndpoint()
    {
        Put("api/v1/roles/{RoleId}/permissions");
        Description(x => x.WithTags("Role Management").WithSummary("Assign permissions to a role"));
    }

    public override async Task HandleAuthorizedAsync(AssignRolePermissionsCommand req, Guid userId, CancellationToken ct)
    {
        IEnumerable<Guid> assignedPermissions = await Mediator.Send(req, ct);
        await SendSuccessAsync(assignedPermissions, "Permissions assigned to role successfully", ct);
    }
}
