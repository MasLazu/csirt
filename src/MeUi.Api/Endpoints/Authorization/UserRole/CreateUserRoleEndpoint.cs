using MeUi.Api.Endpoints;
using MeUi.Application.Features.Authorization.Commands.CreateUserRole;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.Authorization.UserRole;

public class CreateUserRoleEndpoint : BaseEndpoint<CreateUserRoleCommand, Guid>, IPermissionProvider
{
    public static string Permission => "CREATE:USER_ROLE";

    public override void ConfigureEndpoint()
    {
        Post("api/v1/users/{UserId}/roles");
        Description(x => x.WithTags("User Authorization").WithSummary("Assign a role to a user"));
    }

    public override async Task HandleAsync(CreateUserRoleCommand req, CancellationToken ct)
    {
        Guid id = await Mediator.Send(req, ct);
        await SendSuccessAsync(id, "User role assigned successfully", ct);
    }
}
