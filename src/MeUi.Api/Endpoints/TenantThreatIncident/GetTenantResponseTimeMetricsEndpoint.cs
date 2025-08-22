using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantThreatIncident.Queries.GetResponseTimeMetrics;
using MeUi.Application.Models.ThreatIncident;
using MeUi.Application.Interfaces;
using System.Collections.Generic;

namespace MeUi.Api.Endpoints.TenantThreatIncident;

public class GetTenantResponseTimeMetricsEndpoint : BaseTenantAuthorizedEndpoint<GetTenantResponseTimeMetricsQuery, List<ResponseTimeMetricDto>, GetTenantResponseTimeMetricsEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string Permission => "READ:THREAT_INCIDENT";
    public static string TenantPermission => "READ:TENANT_THREAT_INCIDENT";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-incident/response-time-metrics");
        Description(x => x.WithTags("Tenant Threat Incident")
            .WithSummary("Get tenant response time metrics (MTTR)")
            .WithDescription("Retrieves response time metrics and MTTR simulation data for tenant-specific threat incident analysis. Requires READ:TENANT_THREAT_INCIDENT permission."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantResponseTimeMetricsQuery req, Guid userId, CancellationToken ct)
    {
        var metrics = await Mediator.Send(req, ct);
        await SendSuccessAsync(metrics, $"Retrieved {metrics.Count} response time metric data points for tenant {req.TenantId}", ct);
    }
}