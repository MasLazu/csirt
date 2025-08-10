using MeUi.Api.Endpoints;
using MeUi.Application.Features.Tenants.Commands.AssignAsnToTenant;
using MeUi.Application.Features.Tenants.Models;

namespace MeUi.Api.Endpoints.Tenants;

public class AssignAsnToTenantEndpoint : BaseEndpoint<AssignAsnToTenantRequest, string>
{
    public override void ConfigureEndpoint()
    {
        Post("api/v1/tenants/asns/assign");
        Description(x => x.WithTags("Tenants").WithSummary("Assign an ASN to a tenant"));
        // TODO: Add authorization for super admin only
    }

    public override async Task HandleAsync(AssignAsnToTenantRequest req, CancellationToken ct)
    {
        // TODO: Get the current user ID from the JWT token/context
        // For now, using a placeholder - this should be replaced with actual user context
        var assignedByTenantUserId = Guid.NewGuid(); // This should come from the authenticated user context

        var command = new AssignAsnToTenantCommand(
            req.TenantId,
            req.AsnId,
            assignedByTenantUserId);

        await Mediator.Send(command, ct);
        await SendSuccessAsync("ASN assigned successfully", "ASN has been assigned to the tenant", ct);
    }
}