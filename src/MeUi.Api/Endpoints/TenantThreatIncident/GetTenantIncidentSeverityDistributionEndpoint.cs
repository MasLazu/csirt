using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantThreatIncident.Queries.GetIncidentSeverityDistribution;
using MeUi.Application.Models.ThreatIncident;
using MeUi.Application.Interfaces;
using System.Collections.Generic;

namespace MeUi.Api.Endpoints.TenantThreatIncident;

public class GetTenantIncidentSeverityDistributionEndpoint : BaseTenantAuthorizedEndpoint<GetTenantIncidentSeverityDistributionQuery, List<IncidentSeverityDistributionDto>, GetTenantIncidentSeverityDistributionEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string Permission => "READ:THREAT_INCIDENT";
    public static string TenantPermission => "READ:TENANT_THREAT_INCIDENT";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-incident/incident-severity-distribution");
        Description(x => x.WithTags("Tenant Threat Incident")
            .WithSummary("Get tenant incident severity distribution")
            .WithDescription("Retrieves incident severity distribution pie chart data for tenant-specific threat incident analysis. Requires READ:TENANT_THREAT_INCIDENT permission."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantIncidentSeverityDistributionQuery req, Guid userId, CancellationToken ct)
    {
        var distribution = await Mediator.Send(req, ct);
        await SendSuccessAsync(distribution, $"Retrieved incident severity distribution with {distribution.Count} severity levels for tenant {req.TenantId}", ct);
    }
}