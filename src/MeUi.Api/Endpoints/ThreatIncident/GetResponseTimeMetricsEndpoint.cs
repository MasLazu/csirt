using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatIncident.Queries.GetResponseTimeMetrics;
using MeUi.Application.Models.ThreatIncident;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.ThreatIncident;

public class GetResponseTimeMetricsEndpoint : BaseAuthorizedEndpoint<GetResponseTimeMetricsQuery, List<ResponseTimeMetricDto>, GetResponseTimeMetricsEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_INCIDENT";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-incident/response-time-metrics");
        Description(x => x.WithTags("Threat Incident")
            .WithSummary("Get response time metrics")
            .WithDescription("Retrieves response time metrics. Requires READ:THREAT_INCIDENT permission."));
    }

    public override async Task HandleAuthorizedAsync(GetResponseTimeMetricsQuery req, Guid userId, CancellationToken ct)
    {
        var resp = await Mediator.Send(req, ct);
        await SendSuccessAsync(resp, $"Retrieved {resp?.Count ?? 0} metric points", ct);
    }
}
