using MeUi.Api.Endpoints;
using MeUi.Application.Features.Authorization.Queries.GetRolesPaginated;
using MeUi.Application.Features.Authorization.Models;
using MeUi.Application.Models;

namespace MeUi.Api.Endpoints.Authorization;

public class GetRolesEndpoint : BaseEndpoint<GetRolesPaginatedQuery, PaginatedResult<RoleDto>>
{
    public override void ConfigureEndpoint()
    {
        Get("api/v1/roles");
        Description(x => x.WithTags("Role").WithSummary("Get paginated list of roles"));
    }

    public override async Task HandleAsync(GetRolesPaginatedQuery req, CancellationToken ct)
    {
        PaginatedResult<RoleDto> result = await Mediator.Send(req, ct);
        await SendSuccessAsync(result, "Roles retrieved successfully", ct);
    }
}