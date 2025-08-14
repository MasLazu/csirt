using MeUi.Api.Endpoints;
using MeUi.Application.Features.Users.Commands.UpdateUser;

namespace MeUi.Api.Endpoints.Users;

public class UpdateUserEndpoint : BaseAuthorizedEndpoint<UpdateUserCommand, Guid, UpdateUserEndpoint>, MeUi.Application.Interfaces.IPermissionProvider
{
    public static string Permission => "UPDATE:USER";

    public override void ConfigureEndpoint()
    {
        Put("api/v1/users/{id}");
        Description(x => x.WithTags("User Management").WithSummary("Update an existing user"));
    }

    public override async Task HandleAuthorizedAsync(UpdateUserCommand req, Guid userId, CancellationToken ct)
    {
        Guid updatedUserId = await Mediator.Send(req, ct);
        await SendSuccessAsync(updatedUserId, "User updated successfully", ct);
    }
}