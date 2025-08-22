using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantThreatIntelligentOverview.Queries.GetThreatCategoryAnalysis;
using MeUi.Application.Models.ThreatIntelligentOverview;
using MeUi.Application.Interfaces;
using System.Collections.Generic;

namespace MeUi.Api.Endpoints.TenantThreatIntelligentOverview;

public class GetTenantThreatCategoryAnalysisEndpoint : BaseTenantAuthorizedEndpoint<GetTenantThreatCategoryAnalysisQuery, List<ThreatCategoryAnalysisDto>, GetTenantThreatCategoryAnalysisEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string Permission => "READ:THREAT_INTELLIGENT_OVERVIEW";
    public static string TenantPermission => "READ:TENANT_THREAT_INTELLIGENT_OVERVIEW";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-intelligent-overview/threat-category-analysis");
        Description(x => x.WithTags("Tenant Threat Intelligent Overview")
            .WithSummary("Get tenant threat category analysis")
            .WithDescription("Retrieves detailed threat category analysis for tenant-specific threat intelligence overview. Requires READ:TENANT_THREAT_INTELLIGENT_OVERVIEW permission."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantThreatCategoryAnalysisQuery req, Guid userId, CancellationToken ct)
    {
        var analysis = await Mediator.Send(req, ct);
        await SendSuccessAsync(analysis, $"Retrieved {analysis.Count} threat category analysis entries for tenant {req.TenantId}", ct);
    }
}