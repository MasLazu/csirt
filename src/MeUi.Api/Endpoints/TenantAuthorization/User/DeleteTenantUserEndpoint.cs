using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantUsers.Commands.DeleteTenantUser;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.TenantAuthorization.User;

public class DeleteTenantUserEndpoint : BaseTenantAuthorizedEndpointWithoutResponse<DeleteTenantUserCommand, DeleteTenantUserEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string TenantPermission => "DELETE:USER";
    public static string Permission => "DELETE:TENANT_USER";

    public override void ConfigureEndpoint()
    {
        Delete("api/v1/tenants/{tenantId}/users/{id}");
        Description(x => x.WithTags("Tenant User Management").WithSummary("Delete a user within a tenant context"));
    }

    protected override async Task HandleAuthorizedAsync(DeleteTenantUserCommand req, Guid userId, CancellationToken ct)
    {
        await Mediator.Send(req, ct);
        await SendSuccessAsync("Tenant user deleted successfully", ct);
    }
}
