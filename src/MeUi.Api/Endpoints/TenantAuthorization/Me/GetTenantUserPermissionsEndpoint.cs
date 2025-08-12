using FastEndpoints.Security;
using MeUi.Api.Endpoints;
using MeUi.Application.Exceptions;
using MeUi.Application.Features.TenantAuthorization.Queries.GetTenantUserPermissions;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;

namespace MeUi.Api.Endpoints.TenantAuthorization.Me;

public class GetTenantUserPermissionsEndpoint : BaseEndpointWithoutRequest<IEnumerable<PermissionDto>>
{
    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenants/{tenantId}/users/me/permissions");
        Description(x => x.WithTags("Tenant Authorization").WithSummary("Get permissions for current user in tenant context"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        string? sub = User.ClaimValue("sub");

        if (string.IsNullOrEmpty(sub))
        {
            throw new UnauthorizedException("User is not authenticated");
        }

        var req = new GetTenantUserPermissionsQuery
        {
            UserId = Guid.Parse(sub),
        };

        IEnumerable<PermissionDto> permissions = await Mediator.Send(req, ct);
        await SendSuccessAsync(permissions, "Tenant user permissions retrieved successfully", ct);
    }
}