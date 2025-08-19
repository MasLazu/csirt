using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatNetwork.Queries.GetProtocolDistribution;
using MeUi.Application.Models.ThreatNetwork;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.ThreatNetwork;

public class GetProtocolDistributionEndpoint : BaseAuthorizedEndpoint<GetProtocolDistributionQuery, List<ProtocolDistributionDto>, GetProtocolDistributionEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_NETWORK";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-network/protocol-distribution");
        Description(x => x.WithTags("Threat Network").WithSummary("Get protocol distribution").WithDescription("Returns protocol usage counts."));
    }

    public override async Task HandleAuthorizedAsync(GetProtocolDistributionQuery req, Guid userId, CancellationToken ct)
    {
        var resp = await Mediator.Send(req, ct);
        await SendSuccessAsync(resp, $"Retrieved {resp?.Count ?? 0} protocols", ct);
    }
}
