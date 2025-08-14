using MeUi.Api.Endpoints;
using MeUi.Application.Features.Authorization.Queries.GetPermissions;
using MeUi.Application.Models;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.Authorization.Permission;

public class GetPermissionsEndpoint : BaseAuthorizedEndpointWithoutRequest<IEnumerable<PermissionDto>, GetPermissionsEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:PERMISSION";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/authorization/permissions");
        Description(x => x.WithTags("System Authorization").WithSummary("Get all permissions"));
    }

    public override async Task HandleAuthorizedAsync(Guid userId, CancellationToken ct)
    {
        IEnumerable<PermissionDto> permissions = await Mediator.Send(new GetPermissionsQuery(), ct);
        await SendSuccessAsync(permissions, "Permissions retrieved successfully", ct);
    }
}