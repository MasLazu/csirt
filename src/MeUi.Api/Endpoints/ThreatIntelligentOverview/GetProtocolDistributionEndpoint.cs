using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatIntelligentOverview.Queries.GetProtocolDistribution;
using MeUi.Application.Models.ThreatIntelligentOverview;
using MeUi.Application.Interfaces;
using System.Collections.Generic;

namespace MeUi.Api.Endpoints.ThreatIntelligentOverview;

public class GetProtocolDistributionEndpoint : BaseAuthorizedEndpoint<GetProtocolDistributionQuery, List<ProtocolDistributionDto>, GetProtocolDistributionEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_INTELLIGENT_OVERVIEW";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-intelligent-overview/protocol-distribution");
        Description(x => x.WithTags("Threat Intelligent Overview")
            .WithSummary("Get protocol distribution")
            .WithDescription("Retrieves protocol distribution for threat intelligence overview. Requires READ:THREAT_INTELLIGENT_OVERVIEW permission."));
    }

    public override async Task HandleAuthorizedAsync(GetProtocolDistributionQuery req, Guid userId, CancellationToken ct)
    {
        List<ProtocolDistributionDto> protocols = await Mediator.Send(req, ct);
        await SendSuccessAsync(protocols, $"Retrieved {protocols.Count} protocol distributions", ct);
    }
}
