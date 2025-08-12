using MeUi.Api.Endpoints;
using MeUi.Application.Features.Users.Queries.GetUsersPaginated;
using MeUi.Application.Models;

namespace MeUi.Api.Endpoints.Users;


public class GetUsersEndpoint : BaseEndpoint<GetUsersPaginatedQuery, PaginatedDto<UserDto>>
{
    public override void ConfigureEndpoint()
    {
        Get("api/v1/users");
        Description(x => x.WithTags("User Management").WithSummary("Get paginated list of system users with filtering by suspension status and search capabilities"));
    }

    public override async Task HandleAsync(GetUsersPaginatedQuery req, CancellationToken ct)
    {
        PaginatedDto<UserDto> result = await Mediator.Send(req, ct);
        await SendSuccessAsync(result, "Users retrieved successfully", ct);
    }
}