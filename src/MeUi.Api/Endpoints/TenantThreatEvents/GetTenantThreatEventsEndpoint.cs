using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatEvents.Queries.GetTenantThreatEventsPaginated;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;

namespace MeUi.Api.Endpoints.TenantThreatEvents;

public class GetTenantThreatEventsEndpoint : BaseTenantAuthorizedEndpoint<GetTenantThreatEventsPaginatedQuery, PaginatedDto<ThreatEventDto>, GetTenantThreatEventsEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string TenantPermission => "READ:THREAT_EVENT";
    public static string Permission => "READ:TENANT_THREAT_EVENT";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenants/{tenantId}/threat-events");
        Description(x => x.WithTags("Tenant Threat Event Management")
            .WithSummary("Get paginated list of tenant threat events")
            .WithDescription("Retrieves a paginated list of threat events scoped to a specific tenant's ASN registries. Includes time-range filtering, search, and sorting capabilities. Requires READ:TENANT_THREAT_EVENT permission."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantThreatEventsPaginatedQuery req, Guid userId, CancellationToken ct)
    {
        PaginatedDto<ThreatEventDto> threatEvents = await Mediator.Send(req, ct);
        await SendSuccessAsync(threatEvents, $"Retrieved {threatEvents.Items.Count()} tenant threat events successfully", ct);
    }
}
