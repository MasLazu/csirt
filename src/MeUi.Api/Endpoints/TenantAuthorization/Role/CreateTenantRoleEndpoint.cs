using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantAuthorization.Commands.CreateTenantRole;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.TenantAuthorization.Role;

public class CreateTenantRoleEndpoint : BaseEndpoint<CreateTenantRoleCommand, Guid>, ITenantPermissionProvider, IPermissionProvider
{
    public static string TenantPermission => "CREATE:ROLE";
    public static string Permission => "CREATE:TENANT_ROLE";

    public override void ConfigureEndpoint()
    {
        Post("api/v1/tenants/{tenantId}/roles");
        Description(x => x.WithTags("Tenant Role Management").WithSummary("Create a new role within a tenant context"));
    }

    public override async Task HandleAsync(CreateTenantRoleCommand req, CancellationToken ct)
    {
        Guid roleId = await Mediator.Send(req, ct);
        await SendSuccessAsync(roleId, "Tenant role created successfully", ct);
    }
}
