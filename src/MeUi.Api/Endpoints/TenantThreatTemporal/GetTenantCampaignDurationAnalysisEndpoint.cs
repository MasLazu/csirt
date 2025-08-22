using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantThreatTemporal.Queries.GetCampaignDurationAnalysis;
using MeUi.Application.Models.ThreatTemporal;
using MeUi.Application.Interfaces;
using System.Collections.Generic;

namespace MeUi.Api.Endpoints.TenantThreatTemporal;

public class GetTenantCampaignDurationAnalysisEndpoint : BaseTenantAuthorizedEndpoint<GetTenantCampaignDurationAnalysisQuery, List<CampaignDurationDto>, GetTenantCampaignDurationAnalysisEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string Permission => "READ:THREAT_TEMPORAL";
    public static string TenantPermission => "READ:TENANT_THREAT_TEMPORAL";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-temporal/campaign-duration-analysis");
        Description(x => x.WithTags("Tenant Threat Temporal")
            .WithSummary("Get tenant attack campaign duration analysis")
            .WithDescription("Retrieves attack campaign duration analysis table data for tenant-specific temporal intelligence. Requires READ:TENANT_THREAT_TEMPORAL permission."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantCampaignDurationAnalysisQuery req, Guid userId, CancellationToken ct)
    {
        var analysis = await Mediator.Send(req, ct);
        await SendSuccessAsync(analysis, $"Retrieved {analysis.Count} campaign duration analysis entries for tenant {req.TenantId}", ct);
    }
}