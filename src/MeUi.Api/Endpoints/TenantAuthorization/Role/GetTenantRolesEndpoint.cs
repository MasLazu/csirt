using MeUi.Api.Endpoints;
using MeUi.Application.Features.Authorization.Queries.GetRolesPaginated;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;

namespace MeUi.Api.Endpoints.TenantAuthorization.Role;

public class GetTenantRolesEndpoint : BaseEndpointWithoutRequest<IEnumerable<RoleDto>>, ITenantPermissionProvider, IPermissionProvider
{
    public static string TenantPermission => "READ:ROLE";
    public static string Permission => "READ:TENANT_ROLE";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenants/{tenantId}/authorization/roles");
        Description(x => x.WithTags("Tenant Authorization").WithSummary("Get roles available in tenant context"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        // For now, use the system roles query - can be enhanced later to filter tenant-specific roles
        var query = new GetRolesPaginatedQuery { Page = 1, PageSize = 1000 };
        PaginatedDto<RoleDto> pagedResult = await Mediator.Send(query, ct);
        await SendSuccessAsync(pagedResult.Items, "Tenant roles retrieved successfully", ct);
    }
}
