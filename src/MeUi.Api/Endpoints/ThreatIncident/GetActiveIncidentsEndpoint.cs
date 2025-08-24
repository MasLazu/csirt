using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatIncident.Queries.GetActiveIncidents;
using MeUi.Application.Models.ThreatIncident;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.ThreatIncident;

public class GetActiveIncidentsEndpoint : BaseAuthorizedEndpoint<GetActiveIncidentsQuery, List<IncidentSummaryDto>, GetActiveIncidentsEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_INCIDENT";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-incident/active-incidents");
        Description(x => x.WithTags("Threat Incident")
            .WithSummary("Get active incident status board")
            .WithDescription("Retrieves active incidents. Requires READ:THREAT_INCIDENT permission."));
    }

    public override async Task HandleAuthorizedAsync(GetActiveIncidentsQuery req, Guid userId, CancellationToken ct)
    {
        List<IncidentSummaryDto> resp = await Mediator.Send(req, ct);
        await SendSuccessAsync(resp, $"Retrieved {resp?.Count ?? 0} incidents", ct);
    }
}
