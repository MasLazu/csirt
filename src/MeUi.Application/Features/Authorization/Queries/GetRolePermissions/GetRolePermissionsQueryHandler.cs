using Mapster;
using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Features.Authorization.Models;
using MeUi.Domain.Entities;
using MeUi.Application.Exceptions;

namespace MeUi.Application.Features.Authorization.Queries.GetRolePermissions;

public class GetRolePermissionsQueryHandler : IRequestHandler<GetRolePermissionsQuery, IEnumerable<PermissionDto>>
{
    private readonly IRepository<Role> _roleRepository;
    private readonly IRepository<RolePermission> _rolePermissionRepository;
    private readonly IRepository<Permission> _permissionRepository;
    private readonly IRepository<Resource> _resourceRepository;
    private readonly IRepository<Domain.Entities.Action> _actionRepository;

    public GetRolePermissionsQueryHandler(
        IRepository<Role> roleRepository,
        IRepository<RolePermission> rolePermissionRepository,
        IRepository<Permission> permissionRepository,
        IRepository<Resource> resourceRepository,
        IRepository<Domain.Entities.Action> actionRepository)
    {
        _roleRepository = roleRepository;
        _rolePermissionRepository = rolePermissionRepository;
        _permissionRepository = permissionRepository;
        _resourceRepository = resourceRepository;
        _actionRepository = actionRepository;
    }

    public async Task<IEnumerable<PermissionDto>> Handle(GetRolePermissionsQuery request, CancellationToken ct)
    {
        Role role = await _roleRepository.GetByIdAsync(request.RoleId, ct) ??
            throw new NotFoundException($"Role with ID {request.RoleId} not found");

        IEnumerable<RolePermission> rolePermissions = await _rolePermissionRepository.FindAsync(rp => rp.RoleId == request.RoleId, ct);
        IEnumerable<Guid> permissionIds = rolePermissions.Select(rp => rp.PermissionId);
        IEnumerable<Permission> permissions = await _permissionRepository.FindAsync(p => permissionIds.Contains(p.Id), ct);

        var resourceCodes = permissions.Select(p => p.ResourceCode).ToHashSet();
        var actionCodes = permissions.Select(p => p.ActionCode).ToHashSet();

        IEnumerable<Resource> resources = await _resourceRepository.FindAsync(r => resourceCodes.Contains(r.Code), ct);
        IEnumerable<Domain.Entities.Action> actions = await _actionRepository.FindAsync(a => actionCodes.Contains(a.Code), ct);

        foreach (Permission permission in permissions)
        {
            permission.Resource = resources.FirstOrDefault(r => r.Code == permission.ResourceCode);
            permission.Action = actions.FirstOrDefault(a => a.Code == permission.ActionCode);
        }

        return permissions.Adapt<IEnumerable<PermissionDto>>();
    }
}