using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventTimelineAnalytics;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.Analytics;

namespace MeUi.Api.Endpoints.ThreatEvents;

public class GetThreatEventTimelineAnalyticsEndpoint : BaseAuthorizedEndpoint<GetThreatEventTimelineAnalyticsQuery, ThreatEventTimelineAnalyticsDto, GetThreatEventTimelineAnalyticsEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_ANALYTICS";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-events/analytics/timeline");
        Description(x => x.WithTags("Threat Event Analytics")
            .WithSummary("Get threat event timeline analytics")
            .WithDescription("Retrieves time-series analytics for threat events with configurable intervals and filtering. Optimized for TimescaleDB time-bucketing. Requires READ:THREAT_ANALYTICS permission."));
    }

    public override async Task HandleAuthorizedAsync(GetThreatEventTimelineAnalyticsQuery req, Guid userId, CancellationToken ct)
    {
        ThreatEventTimelineAnalyticsDto analytics = await Mediator.Send(req, ct);
        await SendSuccessAsync(analytics, $"Retrieved timeline analytics with {analytics.Timeline.Count} data points", ct);
    }
}
