using MeUi.Api.Endpoints;
using MeUi.Application.Features.Authorization.Models;
using MeUi.Application.Features.Authorization.Queries.GetTenantUserPermissions;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.Authorization;

public class GetTenantUserPermissionsEndpoint : BaseEndpoint<GetTenantUserPermissionsRequest, IEnumerable<PermissionDto>>, ITenantPermissionProvider
{
    public static string Permission => "READ:TENANT_USER_PERMISSIONS";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/users/{UserId}/permissions");
        Description(x => x.WithTags("Tenant Authorization").WithSummary("Get permissions for a tenant user"));
    }

    public override async Task HandleAsync(GetTenantUserPermissionsRequest req, CancellationToken ct)
    {
        var query = new GetTenantUserPermissionsQuery(req.UserId);
        var permissions = await Mediator.Send(query, ct);
        await SendSuccessAsync(permissions, "Tenant user permissions retrieved successfully", ct);
    }
}