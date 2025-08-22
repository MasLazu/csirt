using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantThreatCompliance.Queries.GetExecutiveSummary;
using MeUi.Application.Models.ThreatCompliance;
using MeUi.Application.Interfaces;
using System.Collections.Generic;

namespace MeUi.Api.Endpoints.TenantThreatCompliance;

public class GetTenantExecutiveSummaryEndpoint : BaseTenantAuthorizedEndpoint<GetTenantExecutiveSummaryQuery, List<ExecutiveSummaryDto>, GetTenantExecutiveSummaryEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string Permission => "READ:THREAT_COMPLIANCE";
    public static string TenantPermission => "READ:TENANT_THREAT_COMPLIANCE";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-compliance/executive-summary");
        Description(x => x.WithTags("Tenant Threat Compliance")
            .WithSummary("Get tenant compliance executive summary")
            .WithDescription("Retrieves executive summary for tenant-specific threat compliance analysis. Requires READ:TENANT_THREAT_COMPLIANCE permission."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantExecutiveSummaryQuery req, Guid userId, CancellationToken ct)
    {
        var summary = await Mediator.Send(req, ct);
        await SendSuccessAsync(summary, $"Retrieved {summary.Count} executive summary entries for tenant {req.TenantId}", ct);
    }
}