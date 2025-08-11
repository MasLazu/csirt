using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantUsers.Models;
using MeUi.Application.Features.TenantUsers.Queries.GetTenantUsersPaginated;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;

namespace MeUi.Api.Endpoints.TenantUsers;

public class GetTenantUsersEndpoint : BaseEndpoint<GetTenantUsersPaginatedQuery, PaginatedResult<TenantUserDto>>, ITenantPermissionProvider
{
    public static string Permission => "READ:TENANT_USER";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenants/{TenantId}/users");
        Description(x => x.WithTags("Tenant Users").WithSummary("Get paginated list of tenant users"));
        // TODO: Add authorization for tenant admin or super admin only
    }

    public override async Task HandleAsync(GetTenantUsersPaginatedQuery req, CancellationToken ct)
    {
        var result = await Mediator.Send(req, ct);
        await SendSuccessAsync(result, "Tenant users retrieved successfully", ct);
    }
}