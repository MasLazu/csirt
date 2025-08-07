using Mapster;
using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Features.Authorization.Models;
using MeUi.Domain.Entities;
using MeUi.Application.Exceptions;

namespace MeUi.Application.Features.Authorization.Queries.GetUserPermissions;

public class GetUserPermissionsQueryHandler : IRequestHandler<GetUserPermissionsQuery, IEnumerable<PermissionDto>>
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<UserRole> _userRoleRepository;
    private readonly IRepository<RolePermission> _rolePermissionRepository;
    private readonly IRepository<Permission> _permissionRepository;
    private readonly IRepository<Resource> _resourceRepository;
    private readonly IRepository<Domain.Entities.Action> _actionRepository;

    public GetUserPermissionsQueryHandler(
        IRepository<User> userRepository,
        IRepository<UserRole> userRoleRepository,
        IRepository<RolePermission> rolePermissionRepository,
        IRepository<Permission> permissionRepository,
        IRepository<Resource> resourceRepository,
        IRepository<Domain.Entities.Action> actionRepository)
    {
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;
        _rolePermissionRepository = rolePermissionRepository;
        _permissionRepository = permissionRepository;
        _resourceRepository = resourceRepository;
        _actionRepository = actionRepository;
    }

    public async Task<IEnumerable<PermissionDto>> Handle(GetUserPermissionsQuery request, CancellationToken ct)
    {
        User? user = await _userRepository.GetByIdAsync(request.UserId, ct)
            ?? throw new NotFoundException($"User with ID {request.UserId} not found");

        IEnumerable<UserRole> userRoles = await _userRoleRepository.FindAsync(ur => ur.UserId == request.UserId, ct);
        var roleIds = userRoles.Select(ur => ur.RoleId).ToList();

        IEnumerable<RolePermission> rolePermissions = await _rolePermissionRepository.FindAsync(rp => roleIds.Contains(rp.RoleId), ct);
        var permissionIds = rolePermissions.Select(rp => rp.PermissionId).ToHashSet();

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