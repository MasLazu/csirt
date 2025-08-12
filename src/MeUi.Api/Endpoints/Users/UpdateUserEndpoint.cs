using MeUi.Api.Endpoints;
using MeUi.Application.Features.Users.Commands.UpdateUser;

namespace MeUi.Api.Endpoints.Users;

public class UpdateUserEndpoint : BaseEndpoint<UpdateUserCommand, Guid>
{
    public override void ConfigureEndpoint()
    {
        Put("api/v1/users/{id}");
        Description(x => x.WithTags("User Management").WithSummary("Update an existing user"));
    }

    public override async Task HandleAsync(UpdateUserCommand req, CancellationToken ct)
    {
        Guid userId = await Mediator.Send(req, ct);
        await SendSuccessAsync(userId, "User updated successfully", ct);
    }
}