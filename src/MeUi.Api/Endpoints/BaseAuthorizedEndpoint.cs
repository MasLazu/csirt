using FastEndpoints;
using FastEndpoints.Security;
using MediatR;
using MeUi.Api.Models;
using MeUi.Application.Features.Authorization.Queries.CheckPermission;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints;

public abstract class BaseAuthorizedEndpoint<TRequest, TResponse, TPermissionProvider> : BaseEndpoint<TRequest, ApiResponse<TResponse>>
    where TRequest : notnull, new()
    where TPermissionProvider : notnull, IPermissionProvider
{
    public override async Task HandleAsync(TRequest req, CancellationToken ct)
    {
        Guid userId = GetUserId();
        string permission = TPermissionProvider.Permission;
        bool allowed = await Mediator.Send(new CheckPermissionQuery(userId, permission), ct);
        if (!allowed)
            throw new ForbiddenException($"Missing permission: {permission}");

        await HandleAuthorizedAsync(req, userId, ct);
    }

    protected abstract Task HandleAuthorizedAsync(TRequest req, Guid userId, CancellationToken ct);

    private Guid GetUserId()
    {
        string? sub = User.ClaimValue("sub");
        if (string.IsNullOrWhiteSpace(sub) || !Guid.TryParse(sub, out var id))
        {
            throw new UnauthorizedException("User is not authenticated");
        }
        return id;
    }
}
