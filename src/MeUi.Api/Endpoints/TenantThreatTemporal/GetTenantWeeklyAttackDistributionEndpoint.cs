using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantThreatTemporal.Queries.GetWeeklyAttackDistribution;
using MeUi.Application.Models.ThreatTemporal;
using MeUi.Application.Interfaces;
using System.Collections.Generic;

namespace MeUi.Api.Endpoints.TenantThreatTemporal;

public class GetTenantWeeklyAttackDistributionEndpoint : BaseTenantAuthorizedEndpoint<GetTenantWeeklyAttackDistributionQuery, List<DayOfWeekDto>, GetTenantWeeklyAttackDistributionEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string Permission => "READ:THREAT_TEMPORAL";
    public static string TenantPermission => "READ:TENANT_THREAT_TEMPORAL";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-temporal/weekly-attack-distribution");
        Description(x => x.WithTags("Tenant Threat Temporal")
            .WithSummary("Get tenant weekly attack distribution")
            .WithDescription("Retrieves weekly attack distribution pie chart data for tenant-specific temporal intelligence. Requires READ:TENANT_THREAT_TEMPORAL permission."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantWeeklyAttackDistributionQuery req, Guid userId, CancellationToken ct)
    {
        var distribution = await Mediator.Send(req, ct);
        await SendSuccessAsync(distribution, $"Retrieved weekly attack distribution with {distribution.Count} days for tenant {req.TenantId}", ct);
    }
}