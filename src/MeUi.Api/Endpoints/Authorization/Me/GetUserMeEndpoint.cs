using FastEndpoints.Security;
using MeUi.Api.Endpoints;
using MeUi.Application.Exceptions;
using MeUi.Application.Features.Users.Queries.GetUserById;
using MeUi.Application.Models;

namespace MeUi.Api.Endpoints.Authorization.Me;

public class GetUserMeEndpoint : BaseEndpointWithoutRequest<UserDto>
{
    public override void ConfigureEndpoint()
    {
        Get("api/v1/users/me");
        Description(x => x.WithTags("User Management").WithSummary("Get current user profile"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        string? sub = User.ClaimValue("sub");
        if (string.IsNullOrEmpty(sub))
        {
            throw new UnauthorizedException("User is not authenticated");
        }

        var query = new GetUserByIdQuery
        {
            Id = Guid.Parse(sub)
        };

        UserDto user = await Mediator.Send(query, ct) ??
            throw new NotFoundException($"User with ID {sub} not found.");

        await SendSuccessAsync(user, "User profile retrieved successfully", ct);
    }
}
