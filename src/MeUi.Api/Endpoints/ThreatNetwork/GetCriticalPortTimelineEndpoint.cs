using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatNetwork.Queries.GetCriticalPortTimeline;
using MeUi.Application.Models.ThreatNetwork;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.ThreatNetwork;

public class GetCriticalPortTimelineEndpoint : BaseAuthorizedEndpoint<GetCriticalPortTimelineQuery, List<CriticalPortTimePointDto>, GetCriticalPortTimelineEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_NETWORK";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-network/critical-port-timeline");
        Description(x => x.WithTags("Threat Network").WithSummary("Get critical port timeline").WithDescription("Returns time series for critical ports."));
    }

    public override async Task HandleAuthorizedAsync(GetCriticalPortTimelineQuery req, Guid userId, CancellationToken ct)
    {
        var resp = await Mediator.Send(req, ct);
        await SendSuccessAsync(resp, $"Retrieved {resp?.Count ?? 0} time points", ct);
    }
}
