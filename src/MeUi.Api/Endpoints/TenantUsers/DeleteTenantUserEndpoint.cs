using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantUsers.Commands.DeleteTenantUser;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.TenantUsers;

public class DeleteTenantUserEndpoint : BaseEndpointWithoutResponse<DeleteTenantUserCommand>, ITenantPermissionProvider
{
    public static string Permission => "DELETE:TENANT_USER";

    public override void ConfigureEndpoint()
    {
        Delete("api/v1/tenant-users/{Id}");
        Description(x => x.WithTags("Tenant Users").WithSummary("Delete tenant user"));
        // TODO: Add authorization for tenant admin or super admin only
    }

    public override async Task HandleAsync(DeleteTenantUserCommand req, CancellationToken ct)
    {
        await Mediator.Send(req, ct);
        await SendSuccessAsync("Tenant user deleted successfully", ct);
    }
}