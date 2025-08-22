using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantThreatTemporal.Queries.GetPeakActivityHours;
using MeUi.Application.Models.ThreatTemporal;
using MeUi.Application.Interfaces;
using System.Collections.Generic;

namespace MeUi.Api.Endpoints.TenantThreatTemporal;

public class GetTenantPeakActivityHoursEndpoint : BaseTenantAuthorizedEndpoint<GetTenantPeakActivityHoursQuery, List<PeakActivityDto>, GetTenantPeakActivityHoursEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string Permission => "READ:THREAT_TEMPORAL";
    public static string TenantPermission => "READ:TENANT_THREAT_TEMPORAL";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-temporal/peak-activity-hours");
        Description(x => x.WithTags("Tenant Threat Temporal")
            .WithSummary("Get tenant peak activity hours by attack category")
            .WithDescription("Retrieves peak activity hours analysis table data for tenant-specific temporal intelligence. Requires READ:TENANT_THREAT_TEMPORAL permission."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantPeakActivityHoursQuery req, Guid userId, CancellationToken ct)
    {
        var activity = await Mediator.Send(req, ct);
        await SendSuccessAsync(activity, $"Retrieved {activity.Count} peak activity entries for tenant {req.TenantId}", ct);
    }
}