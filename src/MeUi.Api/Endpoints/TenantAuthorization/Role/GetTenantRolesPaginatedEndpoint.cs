using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantAuthorization.Queries.GetTenantRolesPaginated;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;

namespace MeUi.Api.Endpoints.TenantAuthorization.Role;

public class GetTenantRolesPaginatedEndpoint : BaseEndpoint<GetTenantRolesPaginatedQuery, PaginatedDto<RoleDto>>, ITenantPermissionProvider, IPermissionProvider
{
    public static string TenantPermission => "READ:ROLE";
    public static string Permission => "READ:TENANT_ROLE";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenants/{tenantId}/roles");
        Description(x => x.WithTags("Tenant Role Management").WithSummary("Get paginated list of roles within a tenant context with search and sorting"));
    }

    public override async Task HandleAsync(GetTenantRolesPaginatedQuery req, CancellationToken ct)
    {
        PaginatedDto<RoleDto> roles = await Mediator.Send(req, ct);
        await SendSuccessAsync(roles, "Tenant roles retrieved successfully", ct);
    }
}
