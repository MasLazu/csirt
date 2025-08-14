using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventComparativeTimelineAnalytics;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.Analytics;

namespace MeUi.Api.Endpoints.ThreatEvents;

public class GetThreatEventComparativeTimelineAnalyticsEndpoint : BaseAuthorizedEndpoint<GetThreatEventComparativeTimelineAnalyticsQuery, ThreatEventTimelineAnalyticsDto, GetThreatEventComparativeTimelineAnalyticsEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_ANALYTICS";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-events/analytics/timeline/comparative");
        Description(x => x.WithTags("Threat Event Analytics")
            .WithSummary("Get comparative timeline analytics")
            .WithDescription("Returns current vs previous period bucketed counts and trend direction. Requires READ:THREAT_ANALYTICS permission."));
    }

    public override async Task HandleAuthorizedAsync(GetThreatEventComparativeTimelineAnalyticsQuery req, Guid userId, CancellationToken ct)
    {
        ThreatEventTimelineAnalyticsDto result = await Mediator.Send(req, ct);
        await SendSuccessAsync(result, $"Retrieved {result.Timeline.Count} comparative timeline buckets", ct);
    }
}
