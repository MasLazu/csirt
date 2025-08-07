using MeUi.Api.Endpoints;
using MeUi.Application.Features.Users.Commands.DeleteUser;

namespace MeUi.Api.Endpoints.Users;

public class DeleteUserEndpoint : BaseEndpoint<DeleteUserCommand, Guid>
{
    public override void ConfigureEndpoint()
    {
        Delete("api/v1/users/{id}");
        Description(x => x.WithTags("User").WithSummary("Delete a user"));
    }

    public override async Task HandleAsync(DeleteUserCommand req, CancellationToken ct)
    {
        Guid userId = await Mediator.Send(req, ct);
        await SendSuccessAsync(userId, "User deleted successfully", ct);
    }
}