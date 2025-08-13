using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventSourceCountriesTopAnalytics;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.Analytics;

namespace MeUi.Api.Endpoints.ThreatEvents;

public class GetThreatEventSourceCountriesTopAnalyticsEndpoint : BaseEndpoint<GetThreatEventSourceCountriesTopAnalyticsQuery, ThreatEventGeoAnalyticsDto>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_ANALYTICS";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-events/analytics/countries/source/top");
        Description(x => x.WithTags("Threat Event Analytics")
            .WithSummary("Get top source countries analytics")
            .WithDescription("Returns top source countries with counts, percentages, top categories, and top malware families. Requires READ:THREAT_ANALYTICS permission."));
    }

    public override async Task HandleAsync(GetThreatEventSourceCountriesTopAnalyticsQuery req, CancellationToken ct)
    {
        var result = await Mediator.Send(req, ct);
        await SendSuccessAsync(result, $"Retrieved {result.SourceCountries.Count} source countries", ct);
    }
}
