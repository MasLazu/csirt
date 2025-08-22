using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantThreatGeographic.Queries.GetCountryRankings;
using MeUi.Application.Models.ThreatGeographic;
using MeUi.Application.Interfaces;
using System.Collections.Generic;

namespace MeUi.Api.Endpoints.TenantThreatGeographic;

public class GetTenantCountryRankingsEndpoint : BaseTenantAuthorizedEndpoint<GetTenantCountryRankingsQuery, List<CountryRankingDto>, GetTenantCountryRankingsEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string Permission => "READ:THREAT_GEOGRAPHIC";
    public static string TenantPermission => "READ:TENANT_THREAT_GEOGRAPHIC";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-geographic/country-rankings");
        Description(x => x.WithTags("Tenant Threat Geographic")
            .WithSummary("Get tenant country attack rankings")
            .WithDescription("Retrieves ranked list of countries by attack volume for tenant-specific threat intelligence. Requires READ:TENANT_THREAT_GEOGRAPHIC permission."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantCountryRankingsQuery req, Guid userId, CancellationToken ct)
    {
        var rankings = await Mediator.Send(req, ct);
        await SendSuccessAsync(rankings, $"Retrieved {rankings.Count} country rankings for tenant {req.TenantId}", ct);
    }
}