using MeUi.Api.Endpoints;
using MeUi.Application.Features.Tenants.Commands.UpdateTenant;

namespace MeUi.Api.Endpoints.Tenants;

public class UpdateTenantEndpoint : BaseEndpointWithoutResponse<UpdateTenantCommand>
{
    public override void ConfigureEndpoint()
    {
        Put("api/v1/tenants/{id}");
        Description(x => x.WithTags("Tenants").WithSummary("Update an existing tenant"));
        // TODO: Add authorization for super admin only
    }

    public override async Task HandleAsync(UpdateTenantCommand req, CancellationToken ct)
    {
        await Mediator.Send(req, ct);
        await SendSuccessAsync("Tenant updated successfully", ct);
    }
}