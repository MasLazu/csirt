using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantThreatIntelligentOverview.Queries.GetThreatActivityTimeline;
using MeUi.Application.Models.ThreatIntelligentOverview;
using MeUi.Application.Interfaces;
using System.Collections.Generic;

namespace MeUi.Api.Endpoints.TenantThreatIntelligentOverview;

public class GetTenantThreatActivityTimelineEndpoint : BaseTenantAuthorizedEndpoint<GetTenantThreatActivityTimelineQuery, List<TimelineDataPointDto>, GetTenantThreatActivityTimelineEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string Permission => "READ:TENANT_THREAT_INTELLIGENT_OVERVIEW";
    public static string TenantPermission => "READ:THREAT_INTELLIGENT_OVERVIEW";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-intelligent-overview/activity-timeline");
        Description(x => x.WithTags("Tenant Threat Intelligent Overview")
            .WithSummary("Get tenant threat activity timeline")
            .WithDescription("Retrieves threat activity timeline data points for tenant-specific threat intelligence overview. Requires READ:TENANT_THREAT_INTELLIGENT_OVERVIEW permission."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantThreatActivityTimelineQuery req, Guid userId, CancellationToken ct)
    {
        List<TimelineDataPointDto> timeline = await Mediator.Send(req, ct);
        await SendSuccessAsync(timeline, $"Retrieved {timeline.Count} timeline data points for tenant {req.TenantId}", ct);
    }
}