using MeUi.Api.Endpoints;
using MeUi.Application.Features.Authorization.Queries.GetRolesPaginated;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;

namespace MeUi.Api.Endpoints.TenantAuthorization.Role;

public class GetTenantRolesEndpoint : BaseTenantAuthorizedEndpoint<GetRolesPaginatedQuery, IEnumerable<RoleDto>, GetTenantRolesEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string TenantPermission => "READ:ROLE";
    public static string Permission => "READ:TENANT_ROLE";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenants/{tenantId}/authorization/roles");
        Description(x => x.WithTags("Tenant Authorization").WithSummary("Get roles available in tenant context"));
    }

    protected override async Task HandleAuthorizedAsync(GetRolesPaginatedQuery req, Guid userId, CancellationToken ct)
    {
        PaginatedDto<RoleDto> pagedResult = await Mediator.Send(req, ct);
        await SendSuccessAsync(pagedResult.Items, "Tenant roles retrieved successfully", ct);
    }
}
