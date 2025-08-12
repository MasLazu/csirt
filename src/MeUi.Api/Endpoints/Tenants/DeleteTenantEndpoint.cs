using MeUi.Api.Endpoints;
using MeUi.Application.Features.Tenants.Commands.DeleteTenant;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.Tenants;

public class DeleteTenantEndpoint : BaseEndpointWithoutResponse<DeleteTenantCommand>, IPermissionProvider
{
    public static string Permission => "DELETE:TENANT";

    public override void ConfigureEndpoint()
    {
        Delete("api/v1/tenants/{id}");
        Description(x => x.WithTags("Tenant Management").WithSummary("Delete a tenant"));
    }

    public override async Task HandleAsync(DeleteTenantCommand req, CancellationToken ct)
    {
        await Mediator.Send(req, ct);
        await SendSuccessAsync("Tenant deleted successfully", ct);
    }
}