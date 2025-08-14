using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventCategoryTimelineAnalytics;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.Analytics;

namespace MeUi.Api.Endpoints.ThreatEvents;

public class GetThreatEventCategoryTimelineAnalyticsEndpoint : BaseAuthorizedEndpoint<GetThreatEventCategoryTimelineAnalyticsQuery, ThreatEventTimelineAnalyticsDto, GetThreatEventCategoryTimelineAnalyticsEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_ANALYTICS";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-events/analytics/categories/timeline");
        Description(x => x.WithTags("Threat Event Analytics")
            .WithSummary("Get category timeline analytics")
            .WithDescription("Returns per-bucket event counts filtered (optionally) to a specific category with category distribution per bucket. Requires READ:THREAT_ANALYTICS permission."));
    }

    public override async Task HandleAuthorizedAsync(GetThreatEventCategoryTimelineAnalyticsQuery req, Guid userId, CancellationToken ct)
    {
        ThreatEventTimelineAnalyticsDto result = await Mediator.Send(req, ct);
        await SendSuccessAsync(result, $"Retrieved {result.Timeline.Count} category timeline buckets", ct);
    }
}
