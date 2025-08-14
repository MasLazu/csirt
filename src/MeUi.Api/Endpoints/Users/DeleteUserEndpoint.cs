using MeUi.Api.Endpoints;
using MeUi.Application.Features.Users.Commands.DeleteUser;

namespace MeUi.Api.Endpoints.Users;

public class DeleteUserEndpoint : BaseAuthorizedEndpoint<DeleteUserCommand, Guid, DeleteUserEndpoint>, MeUi.Application.Interfaces.IPermissionProvider
{
    public static string Permission => "DELETE:USER";

    public override void ConfigureEndpoint()
    {
        Delete("api/v1/users/{id}");
        Description(x => x.WithTags("User Management").WithSummary("Delete a user"));
    }

    public override async Task HandleAuthorizedAsync(DeleteUserCommand req, Guid userId, CancellationToken ct)
    {
        Guid deletedUserId = await Mediator.Send(req, ct);
        await SendSuccessAsync(deletedUserId, "User deleted successfully", ct);
    }
}