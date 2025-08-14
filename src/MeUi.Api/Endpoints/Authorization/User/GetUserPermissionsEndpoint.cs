using MeUi.Api.Endpoints;
using MeUi.Application.Features.Authorization.Queries.GetSpecificUserPermissions;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;

namespace MeUi.Api.Endpoints.Authorization.User;

public class GetUserPermissionsEndpoint : BaseAuthorizedEndpoint<GetSpecificUserPermissionsQuery, IEnumerable<PermissionDto>, GetUserPermissionsEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:USER_PERMISSIONS";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/users/{userId}/permissions");
        Description(x => x.WithTags("User Authorization").WithSummary("Get permissions for a specific user"));
    }
    public override async Task HandleAuthorizedAsync(GetSpecificUserPermissionsQuery req, Guid userId, CancellationToken ct)
    {
        IEnumerable<PermissionDto> permissions = await Mediator.Send(req, ct);
        await SendSuccessAsync(permissions, "User permissions retrieved successfully", ct);
    }
}
