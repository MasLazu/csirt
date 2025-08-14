using MeUi.Api.Endpoints;
using MeUi.Application.Features.Authorization.Commands.UpdateRole;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.Authorization.Role;


public class UpdateRoleEndpoint : BaseAuthorizedEndpoint<UpdateRoleCommand, Guid, UpdateRoleEndpoint>, IPermissionProvider
{
    public static string Permission => "UPDATE:ROLE";

    public override void ConfigureEndpoint()
    {
        Put("api/v1/authorization/roles/{id}");
        Description(x => x.WithTags("System Authorization").WithSummary("Update a role"));
    }

    public override async Task HandleAuthorizedAsync(UpdateRoleCommand req, Guid userId, CancellationToken ct)
    {
        Guid roleId = await Mediator.Send(req, ct);
        await SendSuccessAsync(roleId, "Role updated successfully", ct);
    }
}