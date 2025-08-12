using FastEndpoints.Security;
using MeUi.Api.Endpoints;
using MeUi.Application.Features.Authorization.Queries.GetUserRoles;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;
using MeUi.Application.Exceptions;

namespace MeUi.Api.Endpoints.Authorization.Me;

public class GetUserRolesMeEndpoint : BaseEndpointWithoutRequest<IEnumerable<RoleDto>>
{
    public override void ConfigureEndpoint()
    {
        Get("api/v1/users/me/roles");
        Description(x => x.WithTags("User Authorization").WithSummary("Get my roles"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        string? sub = User.ClaimValue("sub");
        if (string.IsNullOrEmpty(sub))
        {
            throw new UnauthorizedException("User is not authenticated");
        }

        var query = new GetUserRolesQuery
        {
            UserId = Guid.Parse(sub)
        };

        IEnumerable<RoleDto> roles = await Mediator.Send(query, ct);
        await SendSuccessAsync(roles, "User roles retrieved successfully", ct);
    }
}
