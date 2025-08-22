using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantThreatTemporal.Queries.GetWeekdayWeekendTrends;
using MeUi.Application.Models.ThreatTemporal;
using MeUi.Application.Interfaces;
using System.Collections.Generic;

namespace MeUi.Api.Endpoints.TenantThreatTemporal;

public class GetTenantWeekdayWeekendTrendsEndpoint : BaseTenantAuthorizedEndpoint<GetTenantWeekdayWeekendTrendsQuery, List<TimeSeriesPointDto>, GetTenantWeekdayWeekendTrendsEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string Permission => "READ:THREAT_TEMPORAL";
    public static string TenantPermission => "READ:TENANT_THREAT_TEMPORAL";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-temporal/weekday-weekend-trends");
        Description(x => x.WithTags("Tenant Threat Temporal")
            .WithSummary("Get tenant weekday vs weekend attack trends")
            .WithDescription("Retrieves weekday vs weekend attack trends time series data for tenant-specific temporal intelligence. Requires READ:TENANT_THREAT_TEMPORAL permission."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantWeekdayWeekendTrendsQuery req, Guid userId, CancellationToken ct)
    {
        var trends = await Mediator.Send(req, ct);
        await SendSuccessAsync(trends, $"Retrieved {trends.Count} weekday/weekend trend data points for tenant {req.TenantId}", ct);
    }
}