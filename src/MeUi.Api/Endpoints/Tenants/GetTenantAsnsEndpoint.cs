using MeUi.Api.Endpoints;
using MeUi.Application.Features.Tenants.Models;
using MeUi.Application.Features.Tenants.Queries.GetTenantAsns;

namespace MeUi.Api.Endpoints.Tenants;

public class GetTenantAsnsRequest
{
    public Guid TenantId { get; set; }
}

public class GetTenantAsnsEndpoint : BaseEndpoint<GetTenantAsnsRequest, IEnumerable<AsnAssignmentDto>>
{
    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenants/{tenantId}/asns");
        Description(x => x.WithTags("Tenants").WithSummary("Get ASNs assigned to a tenant"));
        // TODO: Add authorization for super admin or tenant admin
    }

    public override async Task HandleAsync(GetTenantAsnsRequest req, CancellationToken ct)
    {
        var query = new GetTenantAsnsQuery(req.TenantId);
        var result = await Mediator.Send(query, ct);
        await SendSuccessAsync(result, "Tenant ASNs retrieved successfully", ct);
    }
}