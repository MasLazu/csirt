using MeUi.Api.Endpoints;
using MeUi.Application.Features.Authorization.Commands.CreateRole;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.Authorization.Role;

public class CreateRoleEndpoint : BaseAuthorizedEndpoint<CreateRoleCommand, Guid, CreateRoleEndpoint>, IPermissionProvider
{
    public static string Permission => "CREATE:ROLE";

    public override void ConfigureEndpoint()
    {
        Post("api/v1/roles");
        Description(x => x.WithTags("Role Management").WithSummary("Create a new role"));
    }

    public override async Task HandleAuthorizedAsync(CreateRoleCommand req, Guid userId, CancellationToken ct)
    {
        Guid roleId = await Mediator.Send(req, ct);
        await SendSuccessAsync(roleId, "Role created successfully", ct);
    }
}