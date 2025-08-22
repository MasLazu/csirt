using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantThreatTemporal.Queries.GetHourDayHeatmap;
using MeUi.Application.Models.ThreatTemporal;
using MeUi.Application.Interfaces;
using System.Collections.Generic;

namespace MeUi.Api.Endpoints.TenantThreatTemporal;

public class GetTenantHourDayHeatmapEndpoint : BaseTenantAuthorizedEndpoint<GetTenantHourDayHeatmapQuery, List<HourDayHeatmapDto>, GetTenantHourDayHeatmapEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string Permission => "READ:THREAT_TEMPORAL";
    public static string TenantPermission => "READ:TENANT_THREAT_TEMPORAL";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-temporal/hour-day-heatmap");
        Description(x => x.WithTags("Tenant Threat Temporal")
            .WithSummary("Get tenant threat activity heatmap")
            .WithDescription("Retrieves threat activity heatmap data (hour vs day) for tenant-specific temporal intelligence. Requires READ:TENANT_THREAT_TEMPORAL permission."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantHourDayHeatmapQuery req, Guid userId, CancellationToken ct)
    {
        var heatmap = await Mediator.Send(req, ct);
        await SendSuccessAsync(heatmap, $"Retrieved {heatmap.Count} heatmap data points for tenant {req.TenantId}", ct);
    }
}