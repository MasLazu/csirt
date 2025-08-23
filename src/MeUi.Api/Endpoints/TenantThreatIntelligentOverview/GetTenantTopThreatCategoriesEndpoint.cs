using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantThreatIntelligentOverview.Queries.GetTopThreatCategories;
using MeUi.Application.Models.ThreatIntelligentOverview;
using MeUi.Application.Interfaces;
using System.Collections.Generic;

namespace MeUi.Api.Endpoints.TenantThreatIntelligentOverview;

public class GetTenantTopThreatCategoriesEndpoint : BaseTenantAuthorizedEndpoint<GetTenantTopThreatCategoriesQuery, List<TopCategoryDto>, GetTenantTopThreatCategoriesEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string Permission => "READ:TENANT_THREAT_INTELLIGENT_OVERVIEW";
    public static string TenantPermission => "READ:THREAT_INTELLIGENT_OVERVIEW";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-intelligent-overview/top-threat-categories");
        Description(x => x.WithTags("Tenant Threat Intelligent Overview")
            .WithSummary("Get tenant top threat categories")
            .WithDescription("Retrieves top threat categories for tenant-specific threat intelligence overview. Requires READ:TENANT_THREAT_INTELLIGENT_OVERVIEW permission."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantTopThreatCategoriesQuery req, Guid userId, CancellationToken ct)
    {
        List<TopCategoryDto> categories = await Mediator.Send(req, ct);
        await SendSuccessAsync(categories, $"Retrieved {categories.Count} top threat categories for tenant {req.TenantId}", ct);
    }
}