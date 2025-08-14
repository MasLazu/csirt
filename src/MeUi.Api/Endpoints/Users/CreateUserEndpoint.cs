using MeUi.Api.Endpoints;
using MeUi.Application.Features.Users.Commands.CreateUser;

namespace MeUi.Api.Endpoints.Users;

public class CreateUserEndpoint : BaseAuthorizedEndpoint<CreateUserCommand, Guid, CreateUserEndpoint>, MeUi.Application.Interfaces.IPermissionProvider
{
    public static string Permission => "CREATE:USER";

    public override void ConfigureEndpoint()
    {
        Post("api/v1/users");
        Description(x => x.WithTags("User Management").WithSummary("Create a new user"));
    }

    public override async Task HandleAuthorizedAsync(CreateUserCommand req, Guid userId, CancellationToken ct)
    {
        Guid createdUserId = await Mediator.Send(req, ct);
        await SendSuccessAsync(createdUserId, "User created successfully", ct);
    }
}