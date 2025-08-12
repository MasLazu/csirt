using MeUi.Api.Endpoints;
using MeUi.Application.Features.Tenants.Commands.CreateTenant;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.Tenants;

public class CreateTenantEndpoint : BaseEndpoint<CreateTenantCommand, Guid>, IPermissionProvider
{
    public static string Permission => "CREATE:TENANT";

    public override void ConfigureEndpoint()
    {
        Post("api/v1/tenants");
        Description(x => x.WithTags("Tenant Management").WithSummary("Create a new tenant"));
    }

    public override async Task HandleAsync(CreateTenantCommand req, CancellationToken ct)
    {
        Guid tenantId = await Mediator.Send(req, ct);
        await SendSuccessAsync(tenantId, "Tenant created successfully", ct);
    }
}