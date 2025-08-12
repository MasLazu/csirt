using MeUi.Api.Endpoints;
using MeUi.Application.Features.Authorization.Queries.GetUserRoles;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;

namespace MeUi.Api.Endpoints.Authorization.UserRole;

public class GetUserRolesEndpoint : BaseEndpoint<GetUserRolesQuery, IEnumerable<RoleDto>>, IPermissionProvider
{
    public static string Permission => "READ:USER_ROLE";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/users/{userId}/roles");
        Description(x => x.WithTags("User Authorization").WithSummary("Get roles assigned to a user"));
    }

    public override async Task HandleAsync(GetUserRolesQuery req, CancellationToken ct)
    {
        IEnumerable<RoleDto> roles = await Mediator.Send(req, ct);
        await SendSuccessAsync(roles, "User roles retrieved successfully", ct);
    }
}
