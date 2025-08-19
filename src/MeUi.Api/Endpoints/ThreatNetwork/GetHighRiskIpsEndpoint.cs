using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatNetwork.Queries.GetHighRiskIps;
using MeUi.Application.Models.ThreatNetwork;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.ThreatNetwork;

public class GetHighRiskIpsEndpoint : BaseAuthorizedEndpoint<GetHighRiskIpsQuery, List<HighRiskIpDto>, GetHighRiskIpsEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_NETWORK";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-network/high-risk-ips");
        Description(x => x.WithTags("Threat Network").WithSummary("Get high risk IPs").WithDescription("Returns top IPs by attack score."));
    }

    public override async Task HandleAuthorizedAsync(GetHighRiskIpsQuery req, Guid userId, CancellationToken ct)
    {
        var resp = await Mediator.Send(req, ct);
        await SendSuccessAsync(resp, $"Retrieved {resp?.Count ?? 0} IPs", ct);
    }
}
