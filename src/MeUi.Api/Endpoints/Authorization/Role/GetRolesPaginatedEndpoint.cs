using MeUi.Api.Endpoints;
using MeUi.Application.Features.Authorization.Queries.GetRolesPaginated;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;

namespace MeUi.Api.Endpoints.Authorization.Role;

public class GetRolesPaginatedEndpoint : BaseAuthorizedEndpoint<GetRolesPaginatedQuery, PaginatedDto<RoleDto>, GetRolesPaginatedEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:ROLE";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/authorization/roles");
        Description(x => x.WithTags("System Authorization").WithSummary("Get paginated list of roles"));
    }

    public override async Task HandleAuthorizedAsync(GetRolesPaginatedQuery req, Guid userId, CancellationToken ct)
    {
        PaginatedDto<RoleDto> result = await Mediator.Send(req, ct);
        await SendSuccessAsync(result, "Roles retrieved successfully", ct);
    }
}