using Mapster;
using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;
using MeUi.Application.Exceptions;
using MeUi.Application.Models;

namespace MeUi.Application.Features.Authorization.Queries.GetSpecificUserPermissions;

public class GetSpecificUserPermissionsQueryHandler : IRequestHandler<GetSpecificUserPermissionsQuery, IEnumerable<PermissionDto>>
{
    private readonly IRepository<UserRole> _userRoleRepository;
    private readonly IRepository<RolePermission> _rolePermissionRepository;
    private readonly IRepository<Permission> _permissionRepository;
    private readonly IRepository<Resource> _resourceRepository;
    private readonly IRepository<Domain.Entities.Action> _actionRepository;

    public GetSpecificUserPermissionsQueryHandler(
        IRepository<UserRole> userRoleRepository,
        IRepository<RolePermission> rolePermissionRepository,
        IRepository<Permission> permissionRepository,
        IRepository<Resource> resourceRepository,
        IRepository<Domain.Entities.Action> actionRepository)
    {
        _userRoleRepository = userRoleRepository;
        _rolePermissionRepository = rolePermissionRepository;
        _permissionRepository = permissionRepository;
        _resourceRepository = resourceRepository;
        _actionRepository = actionRepository;
    }

    public async Task<IEnumerable<PermissionDto>> Handle(GetSpecificUserPermissionsQuery request, CancellationToken ct)
    {
        IEnumerable<Guid> userRoleIds = await _userRoleRepository.FindAsync(
            ur => ur.UserId == request.UserId, ur => ur.RoleId, ct);

        IEnumerable<Guid> userPermissions = await _rolePermissionRepository.FindAsync(
            rp => userRoleIds.Contains(rp.RoleId), rp => rp.PermissionId, ct);

        IEnumerable<Permission> permissions = await _permissionRepository.FindAsync(
            p => userPermissions.Contains(p.Id), ct: ct);

        IEnumerable<Resource> resources = await _resourceRepository.FindAsync(
            r => permissions.Select(p => p.ResourceCode).Contains(r.Code), ct: ct);

        IEnumerable<Domain.Entities.Action> actions = await _actionRepository.FindAsync(
            a => permissions.Select(p => p.ActionCode).Contains(a.Code), ct: ct);

        foreach (Permission permission in permissions)
        {
            permission.Resource = resources.FirstOrDefault(r => r.Code == permission.ResourceCode);
            permission.Action = actions.FirstOrDefault(a => a.Code == permission.ActionCode);
        }

        return permissions.Adapt<IEnumerable<PermissionDto>>();
    }
}
