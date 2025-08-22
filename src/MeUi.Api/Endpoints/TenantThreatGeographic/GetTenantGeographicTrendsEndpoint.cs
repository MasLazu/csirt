using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantThreatGeographic.Queries.GetGeographicTrends;
using MeUi.Application.Models.ThreatGeographic;
using MeUi.Application.Interfaces;
using System.Collections.Generic;

namespace MeUi.Api.Endpoints.TenantThreatGeographic;

public class GetTenantGeographicTrendsEndpoint : BaseTenantAuthorizedEndpoint<GetTenantGeographicTrendsQuery, List<GeographicTrendDto>, GetTenantGeographicTrendsEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string Permission => "READ:THREAT_GEOGRAPHIC";
    public static string TenantPermission => "READ:TENANT_THREAT_GEOGRAPHIC";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-geographic/trends");
        Description(x => x.WithTags("Tenant Threat Geographic")
            .WithSummary("Get tenant geographic attack trends over time")
            .WithDescription("Retrieves time series data of geographic attack trends for tenant-specific threat intelligence. Requires READ:TENANT_THREAT_GEOGRAPHIC permission."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantGeographicTrendsQuery req, Guid userId, CancellationToken ct)
    {
        var trends = await Mediator.Send(req, ct);
        await SendSuccessAsync(trends, $"Retrieved {trends.Count} geographic trend data points for tenant {req.TenantId}", ct);
    }
}