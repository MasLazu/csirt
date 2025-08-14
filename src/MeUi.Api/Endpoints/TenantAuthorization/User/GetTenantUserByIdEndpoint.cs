using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantUsers.Queries.GetTenantUserById;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;

namespace MeUi.Api.Endpoints.TenantAuthorization.User;

public class GetTenantUserByIdEndpoint : BaseTenantAuthorizedEndpoint<GetTenantUserByIdQuery, TenantUserDto, GetTenantUserByIdEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string TenantPermission => "READ:USER";
    public static string Permission => "READ:TENANT_USER";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenants/{tenantId}/users/{id}");
        Description(x => x.WithTags("Tenant User Management").WithSummary("Get a specific user by ID within a tenant context"));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantUserByIdQuery req, Guid userId, CancellationToken ct)
    {
        TenantUserDto user = await Mediator.Send(req, ct);
        await SendSuccessAsync(user, "Tenant user retrieved successfully", ct);
    }
}
