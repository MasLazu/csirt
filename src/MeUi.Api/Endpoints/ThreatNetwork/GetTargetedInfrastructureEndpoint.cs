using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatNetwork.Queries.GetTargetedInfrastructure;
using MeUi.Application.Models.ThreatNetwork;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.ThreatNetwork;

public class GetTargetedInfrastructureEndpoint : BaseAuthorizedEndpoint<GetTargetedInfrastructureQuery, List<TargetedInfrastructureDto>, GetTargetedInfrastructureEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_NETWORK";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-network/most-targeted-infrastructure");
        Description(x => x.WithTags("Threat Network").WithSummary("Get most targeted infrastructure").WithDescription("Returns top targeted destination IPs."));
    }

    public override async Task HandleAuthorizedAsync(GetTargetedInfrastructureQuery req, Guid userId, CancellationToken ct)
    {
        var resp = await Mediator.Send(req, ct);
        await SendSuccessAsync(resp, $"Retrieved {resp?.Count ?? 0} targets", ct);
    }
}
