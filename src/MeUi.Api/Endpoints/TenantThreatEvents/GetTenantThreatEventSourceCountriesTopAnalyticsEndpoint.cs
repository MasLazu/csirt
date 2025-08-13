using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventSourceCountriesTopAnalytics;
using MeUi.Application.Features.ThreatEvents.Queries.GetTenantThreatEventSourceCountriesTopAnalytics;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.Analytics;

namespace MeUi.Api.Endpoints.TenantThreatEvents;

public class GetTenantThreatEventSourceCountriesTopAnalyticsEndpoint : BaseEndpoint<GetTenantThreatEventSourceCountriesTopAnalyticsQuery, ThreatEventGeoAnalyticsDto>, IPermissionProvider
{
    public static string TenantPermission => "READ:THREAT_ANALYTICS";
    public static string Permission => "READ:THREAT_ANALYTICS";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-events/analytics/countries/source/top");
        Description(x => x.WithTags("Tenant Threat Event Analytics")
            .WithSummary("Get tenant top source countries analytics")
            .WithDescription("Returns tenant-scoped top source countries with counts, percentages, top categories, and top malware families."));
    }

    public override async Task HandleAsync(GetTenantThreatEventSourceCountriesTopAnalyticsQuery req, CancellationToken ct)
    {
        var tenantId = Route<Guid>("tenantId");
        var enriched = req with { TenantId = tenantId };
        var result = await Mediator.Send(enriched, ct);
        await SendSuccessAsync(result, $"Retrieved {result.SourceCountries.Count} tenant source countries", ct);
    }
}
