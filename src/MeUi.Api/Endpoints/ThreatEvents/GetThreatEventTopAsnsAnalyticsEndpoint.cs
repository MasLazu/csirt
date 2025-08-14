using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventTopAsnsAnalytics;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.Analytics;

namespace MeUi.Api.Endpoints.ThreatEvents;

public class GetThreatEventTopAsnsAnalyticsEndpoint : BaseAuthorizedEndpoint<GetThreatEventTopAsnsAnalyticsQuery, ThreatEventAsnTopAnalyticsDto, GetThreatEventTopAsnsAnalyticsEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_ANALYTICS";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-events/analytics/asns/top");
        Description(x => x.WithTags("Threat Event Analytics")
            .WithSummary("Get top ASNs analytics")
            .WithDescription("Returns top ASNs within a time range including counts, percentages, top categories, source IP samples, and average risk score. Requires READ:THREAT_ANALYTICS permission."));
    }

    public override async Task HandleAuthorizedAsync(GetThreatEventTopAsnsAnalyticsQuery req, Guid userId, CancellationToken ct)
    {
        ThreatEventAsnTopAnalyticsDto result = await Mediator.Send(req, ct);
        await SendSuccessAsync(result, $"Retrieved {result.Asns.Count} ASN entries", ct);
    }
}
