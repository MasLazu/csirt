using MeUi.Api.Endpoints;
using MeUi.Application.Features.Authorization.Queries.GetPermissions;
using MeUi.Application.Models;

namespace MeUi.Api.Endpoints.Authorization.Permission;

public class GetPermissionsEndpoint : BaseEndpointWithoutRequest<IEnumerable<PermissionDto>>
{
    public override void ConfigureEndpoint()
    {
        Get("api/v1/authorization/permissions");
        Description(x => x.WithTags("System Authorization").WithSummary("Get all permissions"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        IEnumerable<PermissionDto> permissions = await Mediator.Send(new GetPermissionsQuery(), ct);
        await SendSuccessAsync(permissions, "Permissions retrieved successfully", ct);
    }
}