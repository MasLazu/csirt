using MeUi.Api.Endpoints;
using MeUi.Application.Features.Tenants.Commands.CreateTenant;

namespace MeUi.Api.Endpoints.Tenants;

public class CreateTenantEndpoint : BaseEndpoint<CreateTenantCommand, Guid>
{
    public override void ConfigureEndpoint()
    {
        Post("api/v1/tenants");
        Description(x => x.WithTags("Tenants").WithSummary("Create a new tenant"));
        // TODO: Add authorization for super admin only
    }

    public override async Task HandleAsync(CreateTenantCommand req, CancellationToken ct)
    {
        var tenantId = await Mediator.Send(req, ct);
        await SendSuccessAsync(tenantId, "Tenant created successfully", ct);
    }
}