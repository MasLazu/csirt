using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantThreatIntelligentOverview.Queries.GetTopSourceCountries;
using MeUi.Application.Models.ThreatIntelligentOverview;
using MeUi.Application.Interfaces;
using System.Collections.Generic;

namespace MeUi.Api.Endpoints.TenantThreatIntelligentOverview;

public class GetTenantTopSourceCountriesEndpoint : BaseTenantAuthorizedEndpoint<GetTenantTopSourceCountriesQuery, List<TopCountryDto>, GetTenantTopSourceCountriesEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string Permission => "READ:TENANT_THREAT_INTELLIGENT_OVERVIEW";
    public static string TenantPermission => "READ:THREAT_INTELLIGENT_OVERVIEW";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-intelligent-overview/top-source-countries");
        Description(x => x.WithTags("Tenant Threat Intelligent Overview")
            .WithSummary("Get tenant top source countries")
            .WithDescription("Retrieves top source countries for tenant-specific threat intelligence overview. Requires READ:TENANT_THREAT_INTELLIGENT_OVERVIEW permission."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantTopSourceCountriesQuery req, Guid userId, CancellationToken ct)
    {
        List<TopCountryDto> countries = await Mediator.Send(req, ct);
        await SendSuccessAsync(countries, $"Retrieved {countries.Count} top source countries for tenant {req.TenantId}", ct);
    }
}