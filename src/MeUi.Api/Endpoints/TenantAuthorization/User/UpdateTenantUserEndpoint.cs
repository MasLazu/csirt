using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantUsers.Commands.UpdateTenantUser;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.TenantAuthorization.User;

public class UpdateTenantUserEndpoint : BaseTenantAuthorizedEndpointWithoutResponse<UpdateTenantUserCommand, UpdateTenantUserEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string TenantPermission => "UPDATE:USER";
    public static string Permission => "UPDATE:TENANT_USER";

    public override void ConfigureEndpoint()
    {
        Put("api/v1/tenants/{tenantId}/users/{id}");
        Description(x => x.WithTags("Tenant User Management").WithSummary("Update an existing user within a tenant context"));
    }

    protected override async Task HandleAuthorizedAsync(UpdateTenantUserCommand req, Guid userId, CancellationToken ct)
    {
        await Mediator.Send(req, ct);
        await SendSuccessAsync("Tenant user updated successfully", ct);
    }
}
