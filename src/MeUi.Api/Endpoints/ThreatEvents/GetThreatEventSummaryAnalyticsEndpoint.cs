using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventSummaryAnalytics;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.Analytics;

namespace MeUi.Api.Endpoints.ThreatEvents;

public class GetThreatEventSummaryAnalyticsEndpoint : BaseEndpoint<GetThreatEventSummaryAnalyticsQuery, ThreatEventSummaryAnalyticsDto>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_ANALYTICS";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-events/analytics/summary");
        Description(x => x.WithTags("Threat Event Analytics")
            .WithSummary("Get threat event summary analytics")
            .WithDescription("Retrieves comprehensive threat intelligence summary including trends, top threats, and critical alerts. Ideal for executive dashboards. Requires READ:THREAT_ANALYTICS permission."));
    }

    public override async Task HandleAsync(GetThreatEventSummaryAnalyticsQuery req, CancellationToken ct)
    {
        ThreatEventSummaryAnalyticsDto analytics = await Mediator.Send(req, ct);
        await SendSuccessAsync(analytics, $"Retrieved threat summary analytics with {analytics.ThreatCategories.Count} categories and {analytics.CriticalAlerts.Count} alerts", ct);
    }
}
