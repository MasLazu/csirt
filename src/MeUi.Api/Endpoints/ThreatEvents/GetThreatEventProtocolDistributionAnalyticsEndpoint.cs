using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventProtocolDistributionAnalytics;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.Analytics;

namespace MeUi.Api.Endpoints.ThreatEvents;

public class GetThreatEventProtocolDistributionAnalyticsEndpoint : BaseEndpoint<GetThreatEventProtocolDistributionAnalyticsQuery, ThreatEventProtocolDistributionAnalyticsDto>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_ANALYTICS";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-events/analytics/protocols/distribution");
        Description(x => x.WithTags("Threat Event Analytics")
            .WithSummary("Get protocol distribution analytics")
            .WithDescription("Returns distribution of network protocols involved in threat events with top ports & categories. Requires READ:THREAT_ANALYTICS permission."));
    }

    public override async Task HandleAsync(GetThreatEventProtocolDistributionAnalyticsQuery req, CancellationToken ct)
    {
        var result = await Mediator.Send(req, ct);
        await SendSuccessAsync(result, $"Retrieved {result.Protocols.Count} protocol entries", ct);
    }
}
