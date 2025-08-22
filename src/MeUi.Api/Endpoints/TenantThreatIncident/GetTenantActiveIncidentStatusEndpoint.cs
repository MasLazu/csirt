using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantThreatIncident.Queries.GetActiveIncidentStatus;
using MeUi.Application.Models.ThreatIncident;
using MeUi.Application.Interfaces;
using System.Collections.Generic;

namespace MeUi.Api.Endpoints.TenantThreatIncident;

public class GetTenantActiveIncidentStatusEndpoint : BaseTenantAuthorizedEndpoint<GetTenantActiveIncidentStatusQuery, List<IncidentStatusDto>, GetTenantActiveIncidentStatusEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string Permission => "READ:THREAT_INCIDENT";
    public static string TenantPermission => "READ:TENANT_THREAT_INCIDENT";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-incident/active-incident-status");
        Description(x => x.WithTags("Tenant Threat Incident")
            .WithSummary("Get tenant active incident status board")
            .WithDescription("Retrieves active incident status board for tenant-specific threat incident response. Requires READ:TENANT_THREAT_INCIDENT permission."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantActiveIncidentStatusQuery req, Guid userId, CancellationToken ct)
    {
        var incidents = await Mediator.Send(req, ct);
        await SendSuccessAsync(incidents, $"Retrieved {incidents.Count} active incidents for tenant {req.TenantId}", ct);
    }
}