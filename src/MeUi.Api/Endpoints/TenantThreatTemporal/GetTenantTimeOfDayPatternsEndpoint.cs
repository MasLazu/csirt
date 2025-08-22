using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantThreatTemporal.Queries.GetTimeOfDayPatterns;
using MeUi.Application.Models.ThreatTemporal;
using MeUi.Application.Interfaces;
using System.Collections.Generic;

namespace MeUi.Api.Endpoints.TenantThreatTemporal;

public class GetTenantTimeOfDayPatternsEndpoint : BaseTenantAuthorizedEndpoint<GetTenantTimeOfDayPatternsQuery, List<TimePeriodSeriesDto>, GetTenantTimeOfDayPatternsEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string Permission => "READ:THREAT_TEMPORAL";
    public static string TenantPermission => "READ:TENANT_THREAT_TEMPORAL";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-temporal/time-of-day-patterns");
        Description(x => x.WithTags("Tenant Threat Temporal")
            .WithSummary("Get tenant attack patterns by time of day")
            .WithDescription("Retrieves attack patterns by time of day time series data for tenant-specific temporal intelligence. Requires READ:TENANT_THREAT_TEMPORAL permission."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantTimeOfDayPatternsQuery req, Guid userId, CancellationToken ct)
    {
        var patterns = await Mediator.Send(req, ct);
        await SendSuccessAsync(patterns, $"Retrieved {patterns.Count} time-of-day pattern data points for tenant {req.TenantId}", ct);
    }
}