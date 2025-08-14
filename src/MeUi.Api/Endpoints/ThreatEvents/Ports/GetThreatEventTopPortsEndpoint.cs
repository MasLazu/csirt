using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventTopPortsAnalytics;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.ThreatEvents.Ports;

public class GetThreatEventTopPortsEndpoint : BaseAuthorizedEndpoint<GetThreatEventTopPortsAnalyticsQuery, object, GetThreatEventTopPortsEndpoint>, MeUi.Application.Interfaces.IPermissionProvider
{
    public static string Permission => "READ:THREAT_ANALYTICS";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-events/analytics/ports/top");
        Description(x => x.WithTags("Threat Event Analytics")
            .WithSummary("Get top source (and optionally destination) ports involved in threat events for the specified window.")
            .WithDescription("Returns the most frequent ports within the given time range. When IncludeDestination=true, includes destination port list."));
    }

    public override async Task HandleAuthorizedAsync(GetThreatEventTopPortsAnalyticsQuery req, Guid userId, CancellationToken ct)
    {
        object result = await Mediator.Send(new GetThreatEventTopPortsAnalyticsQuery
        {
            StartTime = req.StartTime,
            EndTime = req.EndTime,
            Top = req.Top,
            IncludeDestination = req.IncludeDestination
        }, ct);

        await SendSuccessAsync(result, "Retrieved top ports analytics", ct);
    }
}
