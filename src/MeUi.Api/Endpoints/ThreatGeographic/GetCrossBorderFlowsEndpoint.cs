using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatGeographic.Queries.GetCrossBorderFlows;
using MeUi.Application.Models.ThreatGeographic;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.ThreatGeographic;

public class GetCrossBorderFlowsEndpoint : BaseAuthorizedEndpoint<GetCrossBorderFlowsQuery, List<CrossBorderFlowDto>, GetCrossBorderFlowsEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_GEOGRAPHIC";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-geographic/cross-border-flows");
        Description(x => x.WithTags("Threat Geographic")
            .WithSummary("Get cross-border attack flows")
            .WithDescription("Retrieves source-destination-country flows. Requires READ:THREAT_GEOGRAPHIC permission."));
    }

    public override async Task HandleAuthorizedAsync(GetCrossBorderFlowsQuery req, Guid userId, CancellationToken ct)
    {
        List<CrossBorderFlowDto> resp = await Mediator.Send(req, ct);
        await SendSuccessAsync(resp, $"Retrieved {resp?.Count ?? 0} flows", ct);
    }
}
