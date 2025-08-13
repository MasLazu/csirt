using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventOverviewAnalytics;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.Analytics;

namespace MeUi.Api.Endpoints.ThreatEvents;

public class GetThreatEventOverviewAnalyticsEndpoint : BaseEndpoint<GetThreatEventOverviewAnalyticsQuery, ThreatEventOverviewAnalyticsDto>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_ANALYTICS";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-events/analytics/overview");
        Description(x => x.WithTags("Threat Event Analytics")
            .WithSummary("Get threat event overview analytics")
            .WithDescription("Retrieves lightweight high-level overview metrics (fast load) including core counts and top entities. Requires READ:THREAT_ANALYTICS permission."));
    }

    public override async Task HandleAsync(GetThreatEventOverviewAnalyticsQuery req, CancellationToken ct)
    {
        ThreatEventOverviewAnalyticsDto analytics = await Mediator.Send(req, ct);
        await SendSuccessAsync(analytics, $"Retrieved threat overview analytics with {analytics.TopCategories.Count} categories, {analytics.TopMalwareFamilies.Count} malware families", ct);
    }
}
