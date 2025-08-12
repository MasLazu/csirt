using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantUsers.Commands.UpdateTenantUser;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.TenantUsers;

public class UpdateTenantUserEndpoint : BaseEndpointWithoutResponse<UpdateTenantUserCommand>, ITenantPermissionProvider
{
    public static string TenantPermission => "UPDATE:USER";

    public override void ConfigureEndpoint()
    {
        Put("api/v1/tenants/{tenantId}/users/{userId}");
        Description(x => x.WithTags("Tenant User Management").WithSummary("Update a tenant user"));
    }

    public override async Task HandleAsync(UpdateTenantUserCommand req, CancellationToken ct)
    {
        await Mediator.Send(req, ct);
        await SendSuccessAsync("Tenant user updated successfully", ct);
    }
}