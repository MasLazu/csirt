using MeUi.Api.Endpoints;
using MeUi.Application.Features.Authorization.Commands.DeleteRole;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.Authorization.Role;

public class DeleteRoleEndpoint : BaseAuthorizedEndpoint<DeleteRoleCommand, Guid, DeleteRoleEndpoint>, IPermissionProvider
{
    public static string Permission => "DELETE:ROLE";

    public override void ConfigureEndpoint()
    {
        Delete("api/v1/roles/{id}");
        Description(x => x.WithTags("Role Management").WithSummary("Delete a role"));
    }

    public override async Task HandleAuthorizedAsync(DeleteRoleCommand req, Guid userId, CancellationToken ct)
    {
        Guid roleId = await Mediator.Send(req, ct);
        await SendSuccessAsync(roleId, "Role Deleted successfully", ct);
    }
}