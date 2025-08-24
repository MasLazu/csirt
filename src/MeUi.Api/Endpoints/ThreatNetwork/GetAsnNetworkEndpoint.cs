using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatNetwork.Queries.GetAsnNetwork;
using MeUi.Application.Models.ThreatNetwork;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.ThreatNetwork;

public class GetAsnNetworkEndpoint : BaseAuthorizedEndpoint<GetAsnNetworkQuery, List<AsnNetworkDto>, GetAsnNetworkEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_NETWORK";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-network/asn-analysis");
        Description(x => x.WithTags("Threat Network").WithSummary("Get ASN network analysis").WithDescription("Returns ASN aggregated metrics."));
    }

    public override async Task HandleAuthorizedAsync(GetAsnNetworkQuery req, Guid userId, CancellationToken ct)
    {
        List<AsnNetworkDto> resp = await Mediator.Send(req, ct);
        await SendSuccessAsync(resp, $"Retrieved {resp?.Count ?? 0} ASNs", ct);
    }
}
