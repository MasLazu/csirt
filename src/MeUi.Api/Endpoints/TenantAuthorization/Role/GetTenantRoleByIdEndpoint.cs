using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantAuthorization.Queries.GetTenantRoleById;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;

namespace MeUi.Api.Endpoints.TenantAuthorization.Role;

public class GetTenantRoleByIdEndpoint : BaseEndpoint<GetTenantRoleByIdQuery, RoleDto>, ITenantPermissionProvider, IPermissionProvider
{
    public static string TenantPermission => "READ:ROLE";
    public static string Permission => "READ:TENANT_ROLE";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenants/{tenantId}/roles/{id}");
        Description(x => x.WithTags("Tenant Role Management").WithSummary("Get a specific role by ID within a tenant context"));
    }

    public override async Task HandleAsync(GetTenantRoleByIdQuery req, CancellationToken ct)
    {
        RoleDto role = await Mediator.Send(req, ct);
        await SendSuccessAsync(role, "Tenant role retrieved successfully", ct);
    }
}
