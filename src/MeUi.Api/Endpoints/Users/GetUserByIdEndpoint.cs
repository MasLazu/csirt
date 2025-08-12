using MeUi.Api.Endpoints;
using MeUi.Application.Features.Users.Queries.GetUserById;
using MeUi.Application.Exceptions;
using MeUi.Application.Models;

namespace MeUi.Api.Endpoints.Users;

public class GetUserByIdEndpoint : BaseEndpoint<GetUserByIdQuery, UserDto>
{
    public override void ConfigureEndpoint()
    {
        Get("api/v1/users/{id}");
        Description(x => x.WithTags("User Management").WithSummary("Get user by ID"));
    }

    public override async Task HandleAsync(GetUserByIdQuery req, CancellationToken ct)
    {
        UserDto user = await Mediator.Send(req, ct) ??
            throw new NotFoundException($"User with ID {req.Id} not found.");

        await SendSuccessAsync(user, "User retrieved successfully", ct);
    }
}