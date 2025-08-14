using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantAuthorization.Queries.GetTenantPermissions;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;

namespace MeUi.Api.Endpoints.TenantAuthorization.Permission;

public class GetTenantPermissionsEndpoint : BaseTenantAuthorizedEndpoint<GetTenantPermissionsQuery, IEnumerable<PermissionDto>, GetTenantPermissionsEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string TenantPermission => "READ:PERMISSION";
    public static string Permission => "READ:TENANT_PERMISSION";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenants/{tenantId}/authorization/permissions");
        Description(x => x.WithTags("Tenant Authorization").WithSummary("Get permissions accessible in tenant context"));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantPermissionsQuery req, Guid userId, CancellationToken ct)
    {
        IEnumerable<PermissionDto> permissions = await Mediator.Send(req, ct);
        await SendSuccessAsync(permissions, "Tenant permissions retrieved successfully", ct);
    }
}