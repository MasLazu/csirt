using FastEndpoints.Security;
using MeUi.Api.Endpoints;
using MeUi.Application.Exceptions;
using MeUi.Application.Features.Authorization.Queries.GetUserPermissions;
using MeUi.Application.Models;

namespace MeUi.Api.Endpoints.Authorization.Me;

public class GetUserPermissionsEndpoint : BaseEndpointWithoutRequest<IEnumerable<PermissionDto>>
{
    public override void ConfigureEndpoint()
    {
        Get("api/v1/users/me/permissions");
        Description(x => x.WithTags("User Authorization").WithSummary("Get permissions for current user"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        string? sub = User.ClaimValue("sub");

        if (string.IsNullOrEmpty(sub))
        {
            throw new UnauthorizedException("User is not authenticated");
        }

        var req = new GetUserPermissionsQuery
        {
            UserId = Guid.Parse(sub),
        };

        IEnumerable<PermissionDto> permissions = await Mediator.Send(req, ct);
        await SendSuccessAsync(permissions, "Permissions retrieved successfully", ct);
    }
}