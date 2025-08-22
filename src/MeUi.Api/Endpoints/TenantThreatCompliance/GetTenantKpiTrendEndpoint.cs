using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantThreatCompliance.Queries.GetKpiTrend;
using MeUi.Application.Models.ThreatCompliance;
using MeUi.Application.Interfaces;
using System.Collections.Generic;

namespace MeUi.Api.Endpoints.TenantThreatCompliance;

public class GetTenantKpiTrendEndpoint : BaseTenantAuthorizedEndpoint<GetTenantKpiTrendQuery, List<KpiTrendPointDto>, GetTenantKpiTrendEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string Permission => "READ:THREAT_COMPLIANCE";
    public static string TenantPermission => "READ:TENANT_THREAT_COMPLIANCE";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-compliance/kpi-trend");
        Description(x => x.WithTags("Tenant Threat Compliance")
            .WithSummary("Get tenant compliance KPI trends")
            .WithDescription("Retrieves KPI trend analysis for tenant-specific threat compliance monitoring. Requires READ:TENANT_THREAT_COMPLIANCE permission."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantKpiTrendQuery req, Guid userId, CancellationToken ct)
    {
        var trends = await Mediator.Send(req, ct);
        await SendSuccessAsync(trends, $"Retrieved {trends.Count} KPI trend points for tenant {req.TenantId}", ct);
    }
}