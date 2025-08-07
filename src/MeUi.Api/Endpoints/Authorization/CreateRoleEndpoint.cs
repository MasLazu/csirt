using MeUi.Api.Endpoints;
using MeUi.Application.Features.Authorization.Commands.CreateRole;

namespace MeUi.Api.Endpoints.Authorization;


public class CreateRoleEndpoint : BaseEndpoint<CreateRoleCommand, Guid>
{
    public override void ConfigureEndpoint()
    {
        Post("api/v1/roles");
        Description(x => x.WithTags("Role").WithSummary("Create a new role"));
    }

    public override async Task HandleAsync(CreateRoleCommand req, CancellationToken ct)
    {
        Guid roleId = await Mediator.Send(req, ct);
        await SendSuccessAsync(roleId, "Role created successfully", ct);
    }
}