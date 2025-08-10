using MeUi.Api.Endpoints;
using MeUi.Application.Features.Tenants.Commands.DeleteTenant;

namespace MeUi.Api.Endpoints.Tenants;

public class DeleteTenantEndpoint : BaseEndpointWithoutResponse<DeleteTenantCommand>
{
    public override void ConfigureEndpoint()
    {
        Delete("api/v1/tenants/{id}");
        Description(x => x.WithTags("Tenants").WithSummary("Delete a tenant"));
        // TODO: Add authorization for super admin only
    }

    public override async Task HandleAsync(DeleteTenantCommand req, CancellationToken ct)
    {
        await Mediator.Send(req, ct);
        await SendSuccessAsync("Tenant deleted successfully", ct);
    }
}