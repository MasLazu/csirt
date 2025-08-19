using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatIntelligentOverview.Queries.GetTopTargetedPorts;
using MeUi.Application.Models.ThreatIntelligentOverview;
using MeUi.Application.Interfaces;
using System.Collections.Generic;

namespace MeUi.Api.Endpoints.ThreatIntelligentOverview;

public class GetTopTargetedPortsEndpoint : BaseAuthorizedEndpoint<GetTopTargetedPortsQuery, List<TargetedPortDto>, GetTopTargetedPortsEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_INTELLIGENT_OVERVIEW";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-intelligent-overview/top-targeted-ports");
        Description(x => x.WithTags("Threat Intelligent Overview")
            .WithSummary("Get top targeted ports")
            .WithDescription("Retrieves top targeted ports for threat intelligence overview. Requires READ:THREAT_INTELLIGENT_OVERVIEW permission."));
    }

    public override async Task HandleAuthorizedAsync(GetTopTargetedPortsQuery req, Guid userId, CancellationToken ct)
    {
        var ports = await Mediator.Send(req, ct);
        await SendSuccessAsync(ports, $"Retrieved {ports.Count} top targeted ports", ct);
    }
}
