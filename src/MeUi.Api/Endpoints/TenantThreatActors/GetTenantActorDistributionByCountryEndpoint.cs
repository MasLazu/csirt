using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantThreatActors.Queries.GetActorDistributionByCountry;
using MeUi.Application.Models.ThreatActors;
using MeUi.Application.Interfaces;
using System.Collections.Generic;

namespace MeUi.Api.Endpoints.TenantThreatActors;

public class GetTenantActorDistributionByCountryEndpoint : BaseTenantAuthorizedEndpoint<GetTenantActorDistributionByCountryQuery, List<ActorCountryDistributionDto>, GetTenantActorDistributionByCountryEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string Permission => "READ:THREAT_ACTORS";
    public static string TenantPermission => "READ:TENANT_THREAT_ACTORS";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-actors/distribution-by-country");
        Description(x => x.WithTags("Tenant Threat Actors")
            .WithSummary("Get tenant actor distribution by country")
            .WithDescription("Retrieves threat actor distribution by country for tenant-specific analysis. Requires READ:TENANT_THREAT_ACTORS permission."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantActorDistributionByCountryQuery req, Guid userId, CancellationToken ct)
    {
        var distribution = await Mediator.Send(req, ct);
        await SendSuccessAsync(distribution, $"Retrieved {distribution.Count} country distributions for tenant {req.TenantId}", ct);
    }
}