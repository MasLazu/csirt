using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatIntelligentOverview.Queries.GetHighRiskSourceIps;
using MeUi.Application.Models.ThreatIntelligentOverview;
using MeUi.Application.Interfaces;
using System.Collections.Generic;

namespace MeUi.Api.Endpoints.ThreatIntelligentOverview;

public class GetHighRiskSourceIpsEndpoint : BaseAuthorizedEndpoint<GetHighRiskSourceIpsQuery, List<HighRiskSourceIpDto>, GetHighRiskSourceIpsEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_INTELLIGENT_OVERVIEW";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-intelligent-overview/high-risk-source-ips");
        Description(x => x.WithTags("Threat Intelligent Overview")
            .WithSummary("Get high-risk source IPs")
            .WithDescription("Retrieves high-risk source IPs for threat intelligence overview. Requires READ:THREAT_INTELLIGENT_OVERVIEW permission."));
    }

    public override async Task HandleAuthorizedAsync(GetHighRiskSourceIpsQuery req, Guid userId, CancellationToken ct)
    {
        var ips = await Mediator.Send(req, ct);
        await SendSuccessAsync(ips, $"Retrieved {ips.Count} high-risk source IPs", ct);
    }
}
