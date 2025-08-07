using MeUi.Api.Endpoints;
using MeUi.Application.Features.Users.Queries.GetUsersPaginated;
using MeUi.Application.Features.Users.Models;
using MeUi.Application.Models;

namespace MeUi.Api.Endpoints.Users;


public class GetUsersEndpoint : BaseEndpoint<GetUsersPaginatedQuery, PaginatedResult<UserDto>>
{
    public override void ConfigureEndpoint()
    {
        Get("api/v1/users");
        Description(x => x.WithTags("User").WithSummary("Get paginated list of users"));
    }

    public override async Task HandleAsync(GetUsersPaginatedQuery req, CancellationToken ct)
    {
        PaginatedResult<UserDto> result = await Mediator.Send(req, ct);
        await SendSuccessAsync(result, "Users retrieved successfully", ct);
    }
}