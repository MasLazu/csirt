using FastEndpoints;
using FastEndpoints.Security;
using MediatR;
using MeUi.Api.Models;
using MeUi.Application.Features.Authorization.Queries.CheckTenantPermission;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints;

public abstract class BaseTenantAuthorizedEndpoint<TRequest, TResponse, TPermissionProvider> : BaseEndpoint<TRequest, ApiResponse<TResponse>>
    where TRequest : notnull, new()
    where TPermissionProvider : notnull, ITenantPermissionProvider
{
    public override async Task HandleAsync(TRequest req, CancellationToken ct)
    {
        Guid tenantUserId = GetTenantUserId();
        string permission = TPermissionProvider.TenantPermission;
        bool allowed = await Mediator.Send(new CheckTenantPermissionQuery(tenantUserId, permission), ct);
        if (!allowed)
        {
            throw new ForbiddenException($"Missing permission: {permission}");
        }

        await HandleAuthorizedAsync(req, tenantUserId, ct);
    }

    protected abstract Task HandleAuthorizedAsync(TRequest req, Guid userId, CancellationToken ct);

    private Guid GetTenantUserId()
    {
        string? claim = User.ClaimValue("tenant_user_id") ?? User.ClaimValue("sub");
        if (string.IsNullOrWhiteSpace(claim) || !Guid.TryParse(claim, out var id))
            throw new UnauthorizedException("Tenant user is not authenticated");
        return id;
    }
}
