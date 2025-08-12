using MeUi.Api.Endpoints;
using MeUi.Application.Features.Users.Commands.CreateUser;

namespace MeUi.Api.Endpoints.Users;

public class CreateUserEndpoint : BaseEndpoint<CreateUserCommand, Guid>
{
    public override void ConfigureEndpoint()
    {
        Post("api/v1/users");
        Description(x => x.WithTags("User Management").WithSummary("Create a new user"));
    }

    public override async Task HandleAsync(CreateUserCommand req, CancellationToken ct)
    {
        Guid userId = await Mediator.Send(req, ct);
        await SendSuccessAsync(userId, "User created successfully", ct);
    }
}