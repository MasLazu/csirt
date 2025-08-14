using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventTopIpReputationAnalytics;

namespace MeUi.Api.Endpoints.ThreatEvents.IpReputation;

public class GetThreatEventTopIpReputationEndpoint : BaseAuthorizedEndpoint<GetThreatEventTopIpReputationAnalyticsQuery, object, GetThreatEventTopIpReputationEndpoint>, MeUi.Application.Interfaces.IPermissionProvider
{
    public static string Permission => "READ:THREAT_ANALYTICS";

    public override void ConfigureEndpoint()
    {
        Get("/api/v1/threat-events/analytics/ip-reputation/top");
        Description(x => x.WithTags("Threat Event Analytics")
            .WithSummary("Get top source (and optionally destination) IP addresses by reputation frequency in threat events.")
            .WithDescription("Returns the most frequent source IPs (and destination IPs if requested) participating in threat events within the time window."));
    }

    public override async Task HandleAuthorizedAsync(GetThreatEventTopIpReputationAnalyticsQuery req, Guid userId, CancellationToken ct)
    {
        object result = await Mediator.Send(new GetThreatEventTopIpReputationAnalyticsQuery
        {
            StartTime = req.StartTime,
            EndTime = req.EndTime,
            Top = req.Top,
            IncludeDestination = req.IncludeDestination
        }, ct);

        await SendSuccessAsync(result, "Retrieved top IP reputation analytics", ct);
    }
}
