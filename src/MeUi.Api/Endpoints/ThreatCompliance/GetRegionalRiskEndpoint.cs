using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatCompliance.Queries.GetRegionalRisk;
using MeUi.Application.Models.ThreatCompliance;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.ThreatCompliance;

public class GetRegionalRiskEndpoint : BaseAuthorizedEndpoint<GetRegionalRiskQuery, List<RegionalRiskDto>, GetRegionalRiskEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_COMPLIANCE";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-compliance/regional-risk");
        Description(x => x.WithTags("Threat Compliance")
            .WithSummary("Get regional risk")
            .WithDescription("Retrieves regional risk breakdown for threat compliance. Requires READ:THREAT_COMPLIANCE permission."));
    }

    public override async Task HandleAuthorizedAsync(GetRegionalRiskQuery req, Guid userId, CancellationToken ct)
    {
        List<RegionalRiskDto> resp = await Mediator.Send(req, ct);
        await SendSuccessAsync(resp, $"Retrieved {resp?.Count ?? 0} regions", ct);
    }
}
