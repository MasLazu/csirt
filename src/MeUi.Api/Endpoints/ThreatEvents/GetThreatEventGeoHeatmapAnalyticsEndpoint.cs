using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventGeoHeatmapAnalytics;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.Analytics;

namespace MeUi.Api.Endpoints.ThreatEvents;

public class GetThreatEventGeoHeatmapAnalyticsEndpoint : BaseAuthorizedEndpoint<GetThreatEventGeoHeatmapAnalyticsQuery, ThreatEventGeoAnalyticsDto, GetThreatEventGeoHeatmapAnalyticsEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_ANALYTICS";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-events/analytics/geo/heatmap");
        Description(x => x.WithTags("Threat Event Analytics")
            .WithSummary("Get geo heatmap analytics")
            .WithDescription("Returns geographic distribution of threat event sources (heatmap data). Requires READ:THREAT_ANALYTICS permission."));
    }

    public override async Task HandleAuthorizedAsync(GetThreatEventGeoHeatmapAnalyticsQuery req, Guid userId, CancellationToken ct)
    {
        ThreatEventGeoAnalyticsDto result = await Mediator.Send(req, ct);
        await SendSuccessAsync(result, $"Retrieved geo heatmap with {result.SourceCountries.Count} countries", ct);
    }
}
