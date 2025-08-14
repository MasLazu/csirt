using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantUsers.Queries.GetTenantUsersPaginated;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;

namespace MeUi.Api.Endpoints.TenantAuthorization.User;

public class GetTenantUsersEndpoint : BaseTenantAuthorizedEndpoint<GetTenantUsersPaginatedQuery, PaginatedDto<TenantUserDto>, GetTenantUsersEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string TenantPermission => "READ:USER";
    public static string Permission => "READ:TENANT_USER";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenants/{tenantId}/users");
        Description(x => x.WithTags("Tenant User Management").WithSummary("Get paginated list of users within a tenant with filtering by suspension status and search capabilities"));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantUsersPaginatedQuery req, Guid userId, CancellationToken ct)
    {
        PaginatedDto<TenantUserDto> users = await Mediator.Send(req, ct);
        await SendSuccessAsync(users, "Tenant users retrieved successfully", ct);
    }
}
