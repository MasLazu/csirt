using MeUi.Api.Endpoints;
using MeUi.Application.Features.Tenants.Commands.RemoveAsnFromTenant;
using MeUi.Application.Features.Tenants.Models;

namespace MeUi.Api.Endpoints.Tenants;

public class RemoveAsnFromTenantEndpoint : BaseEndpoint<RemoveAsnFromTenantRequest, string>
{
    public override void ConfigureEndpoint()
    {
        Delete("api/v1/tenants/asns/remove");
        Description(x => x.WithTags("Tenants").WithSummary("Remove an ASN from a tenant"));
        // TODO: Add authorization for super admin only
    }

    public override async Task HandleAsync(RemoveAsnFromTenantRequest req, CancellationToken ct)
    {
        var command = new RemoveAsnFromTenantCommand(req.TenantId, req.AsnId);

        await Mediator.Send(command, ct);
        await SendSuccessAsync("ASN removed successfully", "ASN has been removed from the tenant", ct);
    }
}