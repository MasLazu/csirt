using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantThreatGeographic.Queries.GetCountryCategoryTimeline;
using MeUi.Application.Models.ThreatGeographic;
using MeUi.Application.Interfaces;
using System.Collections.Generic;

namespace MeUi.Api.Endpoints.TenantThreatGeographic;

public class GetTenantCountryCategoryTimelineEndpoint : BaseTenantAuthorizedEndpoint<GetTenantCountryCategoryTimelineQuery, List<CountryCategoryTimelineDto>, GetTenantCountryCategoryTimelineEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string Permission => "READ:THREAT_GEOGRAPHIC";
    public static string TenantPermission => "READ:TENANT_THREAT_GEOGRAPHIC";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-geographic/country-category-timeline");
        Description(x => x.WithTags("Tenant Threat Geographic")
            .WithSummary("Get tenant country-category timeline analysis")
            .WithDescription("Retrieves time series analysis of top countries by attack categories for tenant-specific threat intelligence. Requires READ:TENANT_THREAT_GEOGRAPHIC permission."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantCountryCategoryTimelineQuery req, Guid userId, CancellationToken ct)
    {
        var timeline = await Mediator.Send(req, ct);
        await SendSuccessAsync(timeline, $"Retrieved {timeline.Count} country-category timeline data points for tenant {req.TenantId}", ct);
    }
}