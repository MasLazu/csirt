using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantAuthorization.Commands.DeleteTenantRole;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.TenantAuthorization.Role;

public class DeleteTenantRoleEndpoint : BaseEndpointWithoutResponse<DeleteTenantRoleCommand>, ITenantPermissionProvider, IPermissionProvider
{
    public static string TenantPermission => "DELETE:ROLE";
    public static string Permission => "DELETE:TENANT_ROLE";

    public override void ConfigureEndpoint()
    {
        Delete("api/v1/tenants/{tenantId}/roles/{id}");
        Description(x => x.WithTags("Tenant Role Management").WithSummary("Delete a role within a tenant context"));
    }

    public override async Task HandleAsync(DeleteTenantRoleCommand req, CancellationToken ct)
    {
        await Mediator.Send(req, ct);
        await SendSuccessAsync("Tenant role deleted successfully", ct);
    }
}
