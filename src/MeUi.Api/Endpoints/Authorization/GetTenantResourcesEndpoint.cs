using MeUi.Api.Endpoints;
using MeUi.Application.Features.Authorization.Models;
using MeUi.Application.Features.Authorization.Queries.GetTenantResources;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.Authorization;

public class GetTenantResourcesEndpoint : BaseEndpointWithoutRequest<IEnumerable<ResourceDto>>, ITenantPermissionProvider
{
    public static string Permission => "READ:TENANT_RESOURCE";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/resources");
        Description(x => x.WithTags("Tenant Authorization").WithSummary("Get tenant resources"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var resources = await Mediator.Send(new GetTenantResourcesQuery(), ct);
        await SendSuccessAsync(resources, "Tenant resources retrieved successfully", ct);
    }
}