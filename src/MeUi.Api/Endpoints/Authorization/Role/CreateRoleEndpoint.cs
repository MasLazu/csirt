using MeUi.Api.Endpoints;
using MeUi.Application.Features.Authorization.Commands.CreateRole;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.Authorization.Role;

public class CreateRoleEndpoint : BaseEndpoint<CreateRoleCommand, Guid>, IPermissionProvider
{
    public static string Permission => "CREATE:ROLE";

    public override void ConfigureEndpoint()
    {
        Post("api/v1/authorization/roles");
        Description(x => x.WithTags("System Authorization").WithSummary("Create a new role"));
    }

    public override async Task HandleAsync(CreateRoleCommand req, CancellationToken ct)
    {
        Guid roleId = await Mediator.Send(req, ct);
        await SendSuccessAsync(roleId, "Role created successfully", ct);
    }
}