using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantUsers.Commands.CreateTenantUser;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.TenantAuthorization.User;

public class CreateTenantUserEndpoint : BaseEndpoint<CreateTenantUserCommand, Guid>, ITenantPermissionProvider, IPermissionProvider
{
    public static string TenantPermission => "CREATE:USER";
    public static string Permission => "CREATE:TENANT_USER";

    public override void ConfigureEndpoint()
    {
        Post("api/v1/tenants/{tenantId}/users");
        Description(x => x.WithTags("Tenant User Management").WithSummary("Create a new user within a tenant context"));
    }

    public override async Task HandleAsync(CreateTenantUserCommand req, CancellationToken ct)
    {
        Guid userId = await Mediator.Send(req, ct);
        await SendSuccessAsync(userId, "Tenant user created successfully", ct);
    }
}
