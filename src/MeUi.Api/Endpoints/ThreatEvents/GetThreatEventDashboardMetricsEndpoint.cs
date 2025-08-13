using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventDashboardMetrics;
using MeUi.Application.Models.Analytics;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.ThreatEvents;

public class GetThreatEventDashboardMetricsEndpoint : BaseEndpoint<GetThreatEventDashboardMetricsQuery, ThreatEventDashboardMetricsDto>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_ANALYTICS";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-events/analytics/dashboard");
        Description(d => d.WithTags("Threat Event Analytics")
            .WithSummary("Get dashboard metrics")
            .WithDescription("Returns high-level dashboard metrics for the last 24h & hour."));
    }

    public override async Task HandleAsync(GetThreatEventDashboardMetricsQuery req, CancellationToken ct)
    {
        var result = await Mediator.Send(req, ct);
        await SendSuccessAsync(result, "Retrieved dashboard metrics", ct);
    }
}
