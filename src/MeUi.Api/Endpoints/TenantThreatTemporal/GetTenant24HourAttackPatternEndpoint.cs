using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantThreatTemporal.Queries.Get24HourAttackPattern;
using MeUi.Application.Models.ThreatTemporal;
using MeUi.Application.Interfaces;
using System.Collections.Generic;

namespace MeUi.Api.Endpoints.TenantThreatTemporal;

public class GetTenant24HourAttackPatternEndpoint : BaseTenantAuthorizedEndpoint<GetTenant24HourAttackPatternQuery, List<TimeSeriesPointDto>, GetTenant24HourAttackPatternEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string Permission => "READ:THREAT_TEMPORAL";
    public static string TenantPermission => "READ:TENANT_THREAT_TEMPORAL";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-temporal/24-hour-attack-pattern");
        Description(x => x.WithTags("Tenant Threat Temporal")
            .WithSummary("Get tenant 24-hour attack pattern analysis")
            .WithDescription("Retrieves 24-hour attack pattern time series data for tenant-specific temporal intelligence. Requires READ:TENANT_THREAT_TEMPORAL permission."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenant24HourAttackPatternQuery req, Guid userId, CancellationToken ct)
    {
        var pattern = await Mediator.Send(req, ct);
        await SendSuccessAsync(pattern, $"Retrieved {pattern.Count} 24-hour attack pattern data points for tenant {req.TenantId}", ct);
    }
}