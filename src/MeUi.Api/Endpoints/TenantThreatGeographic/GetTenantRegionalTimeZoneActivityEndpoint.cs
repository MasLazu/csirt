using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantThreatGeographic.Queries.GetRegionalTimeZoneActivity;
using MeUi.Application.Models.ThreatGeographic;
using MeUi.Application.Interfaces;
using System.Collections.Generic;

namespace MeUi.Api.Endpoints.TenantThreatGeographic;

public class GetTenantRegionalTimeZoneActivityEndpoint : BaseTenantAuthorizedEndpoint<GetTenantRegionalTimeZoneActivityQuery, List<RegionalTimeZoneActivityDto>, GetTenantRegionalTimeZoneActivityEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string Permission => "READ:THREAT_GEOGRAPHIC";
    public static string TenantPermission => "READ:TENANT_THREAT_GEOGRAPHIC";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-geographic/regional-timezone-activity");
        Description(x => x.WithTags("Tenant Threat Geographic")
            .WithSummary("Get tenant regional time zone activity analysis")
            .WithDescription("Retrieves attack activity patterns by time zones and regions for tenant-specific threat intelligence. Requires READ:TENANT_THREAT_GEOGRAPHIC permission."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantRegionalTimeZoneActivityQuery req, Guid userId, CancellationToken ct)
    {
        var activities = await Mediator.Send(req, ct);
        await SendSuccessAsync(activities, $"Retrieved {activities.Count} regional time zone activities for tenant {req.TenantId}", ct);
    }
}