using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantAuthorization.Queries.GetTenantResources;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;

namespace MeUi.Api.Endpoints.TenantAuthorization.Resource;

public class GetTenantResourcesEndpoint : BaseTenantAuthorizedEndpoint<GetTenantResourcesQuery, IEnumerable<ResourceDto>, GetTenantResourcesEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string TenantPermission => "READ:RESOURCE";
    public static string Permission => "READ:TENANT_RESOURCE";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenants/{tenantId}/authorization/resources");
        Description(x => x.WithTags("Tenant Authorization").WithSummary("Get resources accessible in tenant context"));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantResourcesQuery req, Guid userId, CancellationToken ct)
    {
        IEnumerable<ResourceDto> resources = await Mediator.Send(req, ct);
        await SendSuccessAsync(resources, "Tenant resources retrieved successfully", ct);
    }
}