using FastEndpoints;
using MediatR;
using MeUi.Api.Models;
using MeUi.Application.Features.Authorization.Queries.CheckPermission;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints;

public abstract class BaseAuthorizedEndpointWithoutResponse<TRequest, TPermissionProvider> : BaseEndpointWithoutResponse<TRequest>
    where TRequest : notnull, new()
    where TPermissionProvider : IPermissionProvider
{
    protected Guid GetUserId()
    {
        string? sub = User.FindFirst("sub")?.Value;
        if (string.IsNullOrWhiteSpace(sub) || !Guid.TryParse(sub, out Guid id))
        {
            throw new UnauthorizedException("User is not authenticated");
        }
        return id;
    }

    public override async Task HandleAsync(TRequest req, CancellationToken ct)
    {
        Guid userId = GetUserId();
        bool allowed = await Mediator.Send(new CheckPermissionQuery(userId, TPermissionProvider.Permission), ct);
        if (!allowed)
        {
            throw new ForbiddenException($"Missing permission: {TPermissionProvider.Permission}");
        }
        await HandleAuthorizedAsync(req, userId, ct);
    }

    public abstract Task HandleAuthorizedAsync(TRequest req, Guid userId, CancellationToken ct);
}
