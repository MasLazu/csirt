using MeUi.Api.Endpoints;
using MeUi.Application.Features.Authorization.Queries.GetRolesPaginated;
using MeUi.Application.Features.TenantAuthorization.Queries.GetTenantRolesPaginated;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;

namespace MeUi.Api.Endpoints.TenantAuthorization.Role;

public class GetTenantRolesEndpoint : BaseTenantAuthorizedEndpoint<GetTenantRolesPaginatedQuery, IEnumerable<TenantRoleDto>, GetTenantRolesEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string TenantPermission => "READ:ROLE";
    public static string Permission => "READ:TENANT_ROLE";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenants/{tenantId}/authorization/roles");
        Description(x => x.WithTags("Tenant Authorization").WithSummary("Get roles available in tenant context"));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantRolesPaginatedQuery req, Guid userId, CancellationToken ct)
    {
        PaginatedDto<TenantRoleDto> pagedResult = await Mediator.Send(req, ct);
        await SendSuccessAsync(pagedResult.Items, "Tenant roles retrieved successfully", ct);
    }
}
