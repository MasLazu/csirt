using MeUi.Api.Endpoints;
using MeUi.Application.Features.Authorization.Queries.GetUserPermissions;
using MeUi.Application.Features.Authorization.Models;

namespace MeUi.Api.Endpoints.Authorization;

public class GetUserPermissionsEndpoint : BaseEndpoint<GetUserPermissionsQuery, IEnumerable<PermissionDto>>
{
    public override void ConfigureEndpoint()
    {
        Get("api/v1/users/{userId}/permissions");
        Description(x => x.WithTags("UserPermission").WithSummary("Get user permissions"));
    }

    public override async Task HandleAsync(GetUserPermissionsQuery req, CancellationToken ct)
    {
        IEnumerable<PermissionDto> permissions = await Mediator.Send(req, ct);
        await SendSuccessAsync(permissions, "Permissions retrieved successfully", ct);
    }
}