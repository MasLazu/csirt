using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantAuthorization.Commands.UpdateTenantRole;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.TenantAuthorization.Role;

public class UpdateTenantRoleEndpoint : BaseEndpointWithoutResponse<UpdateTenantRoleCommand>, ITenantPermissionProvider, IPermissionProvider
{
    public static string TenantPermission => "UPDATE:ROLE";
    public static string Permission => "UPDATE:TENANT_ROLE";

    public override void ConfigureEndpoint()
    {
        Put("api/v1/tenants/{tenantId}/roles/{id}");
        Description(x => x.WithTags("Tenant Role Management").WithSummary("Update an existing role within a tenant context"));
    }

    public override async Task HandleAsync(UpdateTenantRoleCommand req, CancellationToken ct)
    {
        await Mediator.Send(req, ct);
        await SendSuccessAsync("Tenant role updated successfully", ct);
    }
}
