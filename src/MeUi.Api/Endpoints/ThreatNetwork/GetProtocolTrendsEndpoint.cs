using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatNetwork.Queries.GetProtocolTrends;
using MeUi.Application.Models.ThreatNetwork;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.ThreatNetwork;

public class GetProtocolTrendsEndpoint : BaseAuthorizedEndpoint<GetProtocolTrendsQuery, List<ProtocolTrendDto>, GetProtocolTrendsEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_NETWORK";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-network/protocol-trends");
        Description(x => x.WithTags("Threat Network").WithSummary("Get protocol usage trends").WithDescription("Returns time series of protocol usage."));
    }

    public override async Task HandleAuthorizedAsync(GetProtocolTrendsQuery req, Guid userId, CancellationToken ct)
    {
        List<ProtocolTrendDto> resp = await Mediator.Send(req, ct);
        await SendSuccessAsync(resp, $"Retrieved {resp?.Count ?? 0} time points", ct);
    }
}
