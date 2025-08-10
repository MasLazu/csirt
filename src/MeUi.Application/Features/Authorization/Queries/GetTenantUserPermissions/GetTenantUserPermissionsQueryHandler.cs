using Mapster;
using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Features.Authorization.Models;
using MeUi.Application.Exceptions;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.Authorization.Queries.GetTenantUserPermissions;

public class GetTenantUserPermissionsQueryHandler : IRequestHandler<GetTenantUserPermissionsQuery, IEnumerable<PermissionDto>>
{
    private readonly IRepository<TenantUserRole> _tenantUserRoleRepository;
    private readonly IRepository<TenantRolePermission> _tenantRolePermissionRepository;

    public GetTenantUserPermissionsQueryHandler(
        IRepository<TenantUserRole> tenantUserRoleRepository,
        IRepository<TenantRolePermission> tenantRolePermissionRepository)
    {
        _tenantUserRoleRepository = tenantUserRoleRepository;
        _tenantRolePermissionRepository = tenantRolePermissionRepository;
    }

    public async Task<IEnumerable<PermissionDto>> Handle(GetTenantUserPermissionsQuery request, CancellationToken ct)
    {
        IEnumerable<Guid> tenantRole = await _tenantUserRoleRepository
            .FindAsync(tur => tur.TenantUserId == request.UserId, tr => tr.TenantRoleId, ct);

        IEnumerable<TenantRolePermission> permissions = await _tenantRolePermissionRepository.FindAsync(
            rp => tenantRole.Contains(rp.TenantRoleId), ct);

        return permissions.Adapt<IEnumerable<PermissionDto>>();
    }
}