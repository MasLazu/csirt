using MeUi.Api.Endpoints;
using MeUi.Application.Features.Tenants.Commands.BulkAssignAsnsToTenant;
using MeUi.Application.Features.Tenants.Models;

namespace MeUi.Api.Endpoints.Tenants;

public class BulkAssignAsnsToTenantEndpoint : BaseEndpoint<BulkAssignAsnsToTenantRequest, BulkAssignAsnsToTenantResult>
{
    public override void ConfigureEndpoint()
    {
        Post("api/v1/tenants/asns/bulk-assign");
        Description(x => x.WithTags("Tenants").WithSummary("Bulk assign ASNs to a tenant"));
        // TODO: Add authorization for super admin only
    }

    public override async Task HandleAsync(BulkAssignAsnsToTenantRequest req, CancellationToken ct)
    {
        // TODO: Get the current user ID from the JWT token/context
        // For now, using a placeholder - this should be replaced with actual user context
        var assignedByTenantUserId = Guid.NewGuid(); // This should come from the authenticated user context

        var command = new BulkAssignAsnsToTenantCommand(
            req.TenantId,
            req.AsnIds,
            assignedByTenantUserId);

        var result = await Mediator.Send(command, ct);
        await SendSuccessAsync(result, "Bulk ASN assignment completed", ct);
    }
}