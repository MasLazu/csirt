using MeUi.Api.Endpoints;
using MeUi.Application.Features.Tenants.Commands.UpdateTenant;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.Tenants;

public class UpdateTenantEndpoint : BaseAuthorizedEndpointWithoutResponse<UpdateTenantCommand, UpdateTenantEndpoint>, IPermissionProvider
{
    public static string Permission => "UPDATE:TENANT";

    public override void ConfigureEndpoint()
    {
        Put("api/v1/tenants/{id}");
        Description(x => x.WithTags("Tenant Management").WithSummary("Update an existing tenant"));
    }

    public override async Task HandleAuthorizedAsync(UpdateTenantCommand req, Guid userId, CancellationToken ct)
    {
        await Mediator.Send(req, ct);
        await SendSuccessAsync("Tenant updated successfully", ct);
    }
}