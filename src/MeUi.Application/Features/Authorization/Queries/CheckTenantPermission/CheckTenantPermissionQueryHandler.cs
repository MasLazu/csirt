using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.Authorization.Queries.CheckTenantPermission;

public sealed class CheckTenantPermissionQueryHandler : IRequestHandler<CheckTenantPermissionQuery, bool>
{
    private readonly IRepository<TenantUserRole> _tenantUserRoles;

    public CheckTenantPermissionQueryHandler(IRepository<TenantUserRole> tenantUserRoles)
    {
        _tenantUserRoles = tenantUserRoles;
    }

    public async Task<bool> Handle(CheckTenantPermissionQuery request, CancellationToken ct)
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

        return await _tenantUserRoles.ExistsAsync(tur => tur.TenantUserId == request.TenantUserId && tur.TenantRole!.TenantId == request.TenantId && tur.TenantRole!.TenantRolePermissions
            .Any(trp => trp.TenantPermission!.ActionCode.ToUpper() == action && trp.TenantPermission.ResourceCode.ToUpper() == resource), ct);
    }
}
