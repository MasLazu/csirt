using FastEndpoints;
using MediatR;
using MeUi.Api.Models;
using MeUi.Application.Features.Authorization.Queries.CheckTenantPermission;
using MeUi.Application.Features.Authorization.Queries.CheckPermission;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;

namespace MeUi.Api.Endpoints;

public abstract class BaseTenantAuthorizedEndpointWithoutResponse<TRequest, TPermissionProvider> : BaseEndpointWithoutResponse<TRequest>
    where TRequest : notnull, ITenantRequest, new()
    where TPermissionProvider : notnull, ITenantPermissionProvider, IPermissionProvider
{
    public override async Task HandleAsync(TRequest req, CancellationToken ct)
    {
        Guid userId = GetUserId();
        string permission = TPermissionProvider.TenantPermission;
        bool allowed = req.TenantId == GetTenantIdFromClaim() && await Mediator.Send(new CheckTenantPermissionQuery(userId, permission, req.TenantId), ct);

        if (!allowed)
        {
            permission = TPermissionProvider.Permission;
            allowed = await Mediator.Send(new CheckPermissionQuery(userId, permission), ct);
        }

        if (!allowed)
        {
            throw new ForbiddenException($"Missing permission: {permission}");
        }

        await HandleAuthorizedAsync(req, userId, ct);
    }

    protected abstract Task HandleAuthorizedAsync(TRequest req, Guid userId, CancellationToken ct);

    protected Guid GetUserId()
    {
        string? claim = User.FindFirst("sub")?.Value;
        if (string.IsNullOrWhiteSpace(claim) || !Guid.TryParse(claim, out Guid id))
        {
            throw new UnauthorizedException("User is not authenticated");
        }
        return id;
    }

    private Guid? GetTenantIdFromClaim()
    {
        string? claim = User.FindFirst("tenant_id")?.Value;
        if (string.IsNullOrWhiteSpace(claim) || !Guid.TryParse(claim, out Guid id))
        {
            return null;
        }
        return id;
    }
}
