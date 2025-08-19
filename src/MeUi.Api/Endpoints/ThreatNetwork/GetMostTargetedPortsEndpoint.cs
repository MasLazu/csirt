using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatNetwork.Queries.GetMostTargetedPorts;
using MeUi.Application.Models.ThreatNetwork;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.ThreatNetwork;

public class GetMostTargetedPortsEndpoint : BaseAuthorizedEndpoint<GetMostTargetedPortsQuery, List<TargetedPortDto>, GetMostTargetedPortsEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_NETWORK";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-network/most-targeted-ports");
        Description(x => x.WithTags("Threat Network").WithSummary("Get most targeted ports").WithDescription("Returns top targeted destination ports."));
    }

    public override async Task HandleAuthorizedAsync(GetMostTargetedPortsQuery req, Guid userId, CancellationToken ct)
    {
        var resp = await Mediator.Send(req, ct);
        await SendSuccessAsync(resp, $"Retrieved {resp?.Count ?? 0} ports", ct);
    }
}
