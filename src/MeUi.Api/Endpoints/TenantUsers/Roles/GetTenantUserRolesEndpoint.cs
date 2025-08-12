using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantUsers.Queries.GetTenantUserRoles;
using MeUi.Application.Models;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.TenantUsers.Roles;

public class GetTenantUserRolesEndpoint : BaseEndpoint<GetTenantUserRolesQuery, IEnumerable<RoleDto>>, ITenantPermissionProvider
{
    public static string TenantPermission => "READ:TENANT_USER_ROLES";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenants/{TenantId}/users/{UserId}/roles");
        Description(x => x.WithTags("Tenant User Roles").WithSummary("Get roles assigned to a tenant user"));
    }

    public override async Task HandleAsync(GetTenantUserRolesQuery req, CancellationToken ct)
    {
        IEnumerable<RoleDto> result = await Mediator.Send(req, ct);
        await SendSuccessAsync(result, "Tenant user roles retrieved successfully", ct);
    }
}
