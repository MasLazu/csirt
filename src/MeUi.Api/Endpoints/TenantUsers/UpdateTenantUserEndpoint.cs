using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantUsers.Commands.UpdateTenantUser;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.TenantUsers;

public class UpdateTenantUserEndpoint : BaseEndpointWithoutResponse<UpdateTenantUserCommand>, ITenantPermissionProvider
{
    public static string Permission => "UPDATE:TENANT_USER";

    public override void ConfigureEndpoint()
    {
        Put("api/v1/tenant-users/{Id}");
        Description(x => x.WithTags("Tenant Users").WithSummary("Update tenant user"));
        // TODO: Add authorization for tenant admin or super admin only
    }

    public override async Task HandleAsync(UpdateTenantUserCommand req, CancellationToken ct)
    {
        await Mediator.Send(req, ct);
        await SendSuccessAsync("Tenant user updated successfully", ct);
    }
}