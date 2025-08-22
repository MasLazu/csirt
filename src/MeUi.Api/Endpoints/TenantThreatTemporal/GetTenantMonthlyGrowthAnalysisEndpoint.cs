using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantThreatTemporal.Queries.GetMonthlyGrowthAnalysis;
using MeUi.Application.Models.ThreatTemporal;
using MeUi.Application.Interfaces;
using System.Collections.Generic;

namespace MeUi.Api.Endpoints.TenantThreatTemporal;

public class GetTenantMonthlyGrowthAnalysisEndpoint : BaseTenantAuthorizedEndpoint<GetTenantMonthlyGrowthAnalysisQuery, List<MonthlyGrowthDto>, GetTenantMonthlyGrowthAnalysisEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string Permission => "READ:THREAT_TEMPORAL";
    public static string TenantPermission => "READ:TENANT_THREAT_TEMPORAL";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-temporal/monthly-growth-analysis");
        Description(x => x.WithTags("Tenant Threat Temporal")
            .WithSummary("Get tenant monthly growth rate analysis")
            .WithDescription("Retrieves monthly growth rate analysis table data for tenant-specific temporal intelligence. Requires READ:TENANT_THREAT_TEMPORAL permission."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantMonthlyGrowthAnalysisQuery req, Guid userId, CancellationToken ct)
    {
        var analysis = await Mediator.Send(req, ct);
        await SendSuccessAsync(analysis, $"Retrieved {analysis.Count} monthly growth analysis entries for tenant {req.TenantId}", ct);
    }
}