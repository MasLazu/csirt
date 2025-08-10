using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantUsers.Commands.CreateTenantUser;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.TenantUsers;

public class CreateTenantUserEndpoint : BaseEndpoint<CreateTenantUserCommand, Guid>, ITenantPermissionProvider
{
    public static string Permission => "CREATE:TENANT_USER";

    public override void ConfigureEndpoint()
    {
        Post("api/v1/tenant-users");
        Description(x => x.WithTags("Tenant Users").WithSummary("Create a new tenant user"));
        // TODO: Add authorization for tenant admin or super admin only
    }

    public override async Task HandleAsync(CreateTenantUserCommand req, CancellationToken ct)
    {
        var tenantUserId = await Mediator.Send(req, ct);
        await SendSuccessAsync(tenantUserId, "Tenant user created successfully", ct);
    }
}