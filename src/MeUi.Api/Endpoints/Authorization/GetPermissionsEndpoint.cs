using MeUi.Application.Features.Authorization.Queries.GetPermissions;
using MeUi.Application.Features.Authorization.Models;

namespace MeUi.Api.Endpoints.Authorization;

public class GetPermissionsEndpoint : BaseEndpointWithoutRequest<IEnumerable<PermissionDto>>
{
    public override void ConfigureEndpoint()
    {
        Get("api/v1/permissions");
        Description(x => x.WithTags("Permission").WithSummary("Get all permissions"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        IEnumerable<PermissionDto> permissions = await Mediator.Send(new GetPermissionsQuery(), ct);
        await SendSuccessAsync(permissions, "Permissions retrieved successfully", ct);
    }
}