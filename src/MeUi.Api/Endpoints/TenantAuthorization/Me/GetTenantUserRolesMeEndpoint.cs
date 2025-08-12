using FastEndpoints.Security;
using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantAuthorization.Queries.GetMyTenantUserRoles;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;
using MeUi.Application.Exceptions;

namespace MeUi.Api.Endpoints.TenantAuthorization.Me;

public class GetTenantUserRolesMeEndpoint : BaseEndpointWithoutRequest<IEnumerable<TenantRoleDto>>
{
    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenants/{tenantId}/users/me/roles");
        Description(x => x.WithTags("Tenant Authorization").WithSummary("Get my roles in tenant context"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        string? sub = User.ClaimValue("sub");
        if (string.IsNullOrEmpty(sub))
        {
            throw new UnauthorizedException("User is not authenticated");
        }

        var query = new GetMyTenantUserRolesQuery
        {
            TenantUserId = Guid.Parse(sub)
        };

        IEnumerable<TenantRoleDto> roles = await Mediator.Send(query, ct);
        await SendSuccessAsync(roles, "Tenant user roles retrieved successfully", ct);
    }
}
