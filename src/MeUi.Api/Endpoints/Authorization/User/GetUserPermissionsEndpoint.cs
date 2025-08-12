using MeUi.Api.Endpoints;
using MeUi.Application.Features.Authorization.Queries.GetSpecificUserPermissions;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;

namespace MeUi.Api.Endpoints.Authorization.User;

public class GetUserPermissionsEndpoint : BaseEndpoint<GetSpecificUserPermissionsQuery, IEnumerable<PermissionDto>>, IPermissionProvider
{
    public static string Permission => "READ:USER_PERMISSIONS";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/users/{userId}/permissions");
        Description(x => x.WithTags("User Authorization").WithSummary("Get permissions for a specific user"));
    }

    public override async Task HandleAsync(GetSpecificUserPermissionsQuery req, CancellationToken ct)
    {
        IEnumerable<PermissionDto> permissions = await Mediator.Send(req, ct);
        await SendSuccessAsync(permissions, "User permissions retrieved successfully", ct);
    }
}
