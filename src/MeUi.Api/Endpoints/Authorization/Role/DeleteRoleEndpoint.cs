using MeUi.Api.Endpoints;
using MeUi.Application.Features.Authorization.Commands.DeleteRole;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.Authorization.Role;

public class DeleteRoleEndpoint : BaseEndpoint<DeleteRoleCommand, Guid>, IPermissionProvider
{
    public static string Permission => "DELETE:ROLE";

    public override void ConfigureEndpoint()
    {
        Delete("api/v1/authorization/roles/{id}");
        Description(x => x.WithTags("System Authorization").WithSummary("Delete a role"));
    }

    public override async Task HandleAsync(DeleteRoleCommand req, CancellationToken ct)
    {
        Guid roleId = await Mediator.Send(req, ct);
        await SendSuccessAsync(roleId, "Role Deleted successfully", ct);
    }
}