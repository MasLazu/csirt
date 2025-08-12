using MeUi.Api.Endpoints;
using MeUi.Application.Features.Authorization.Commands.DeleteUserRole;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.Authorization.UserRole;

public class DeleteUserRoleEndpoint : BaseEndpoint<DeleteUserRoleCommand, Guid>, IPermissionProvider
{
    public static string Permission => "DELETE:USER_ROLE";

    public override void ConfigureEndpoint()
    {
        Delete("api/v1/users/{UserId}/roles/{RoleId}");
        Description(x => x.WithTags("User Authorization").WithSummary("Delete a user role assignment"));
    }

    public override async Task HandleAsync(DeleteUserRoleCommand req, CancellationToken ct)
    {
        Guid id = await Mediator.Send(req, ct);
        await SendSuccessAsync(id, "User role assignment deleted successfully", ct);
    }
}
