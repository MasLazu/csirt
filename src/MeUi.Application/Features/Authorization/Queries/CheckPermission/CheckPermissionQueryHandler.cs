using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.Authorization.Queries.CheckPermission;

public sealed class CheckPermissionQueryHandler : IRequestHandler<CheckPermissionQuery, bool>
{
    private readonly IRepository<UserRole> _userRoles;

    public CheckPermissionQueryHandler(
        IRepository<UserRole> userRoles)
    {
        _userRoles = userRoles;
    }

    public async Task<bool> Handle(CheckPermissionQuery request, CancellationToken ct)
    {
        string[] parts = request.PermissionCode.Trim().Split(':');
        if (parts.Length != 2)
        {
            return false;
        }

        string action = parts[0].Trim().ToUpperInvariant();
        string resource = parts[1].Trim().ToUpperInvariant();

        if (action.Length == 0 || resource.Length == 0)
        {
            return false;
        }

        return await _userRoles
            .ExistsAsync(ur => ur.UserId == request.UserId && ur.Role!.RolePermissions
            .Any(rp => rp.Permission!.ActionCode.ToUpper() == action && rp.Permission.ResourceCode.ToUpper() == resource), ct);
    }
}
