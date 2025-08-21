using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantThreatIntelligentOverview.Queries.GetExecutiveSummary;
using MeUi.Application.Models.ThreatIntelligentOverview;
using MeUi.Application.Interfaces;
using System.Collections.Generic;

namespace MeUi.Api.Endpoints.TenantThreatIntelligentOverview;

public class GetTenantExecutiveSummaryEndpoint : BaseTenantAuthorizedEndpoint<GetTenantExecutiveSummaryQuery, List<ExecutiveSummaryMetricDto>, GetTenantExecutiveSummaryEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string Permission => "READ:THREAT_INTELLIGENT_OVERVIEW";
    public static string TenantPermission => "READ:TENANT_THREAT_INTELLIGENT_OVERVIEW";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-intelligent-overview/executive-summary");
        Description(x => x.WithTags("Tenant Threat Intelligent Overview")
            .WithSummary("Get tenant executive summary metrics")
            .WithDescription("Retrieves high-level executive summary metrics for tenant-specific threat intelligence overview. Requires READ:TENANT_THREAT_INTELLIGENT_OVERVIEW permission."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantExecutiveSummaryQuery req, Guid userId, CancellationToken ct)
    {
        var summary = await Mediator.Send(req, ct);
        await SendSuccessAsync(summary, $"Retrieved {summary.Count} executive summary metrics for tenant {req.TenantId}", ct);
    }
}