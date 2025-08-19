using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatIncident.Queries.GetSeverityDistribution;
using MeUi.Application.Models.ThreatIncident;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.ThreatIncident;

public class GetSeverityDistributionEndpoint : BaseAuthorizedEndpoint<GetSeverityDistributionQuery, List<SeverityDistributionDto>, GetSeverityDistributionEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_INCIDENT";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-incident/severity-distribution");
        Description(x => x.WithTags("Threat Incident")
            .WithSummary("Get incident severity distribution")
            .WithDescription("Retrieves severity distribution. Requires READ:THREAT_INCIDENT permission."));
    }

    public override async Task HandleAuthorizedAsync(GetSeverityDistributionQuery req, Guid userId, CancellationToken ct)
    {
        var resp = await Mediator.Send(req, ct);
        await SendSuccessAsync(resp, $"Retrieved {resp?.Count ?? 0} severity buckets", ct);
    }
}
