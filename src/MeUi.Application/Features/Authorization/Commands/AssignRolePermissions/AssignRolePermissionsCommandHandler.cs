using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;


namespace MeUi.Application.Features.Authorization.Commands.AssignRolePermissions;

public class AssignRolePermissionsCommandHandler : IRequestHandler<AssignRolePermissionsCommand, IEnumerable<Guid>>
{
    private readonly IRepository<Role> _roleRepository;
    private readonly IRepository<Permission> _permissionRepository;
    private readonly IRepository<RolePermission> _rolePermissionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignRolePermissionsCommandHandler(
        IRepository<Role> roleRepository,
        IRepository<Permission> permissionRepository,
        IRepository<RolePermission> rolePermissionRepository,
        IUnitOfWork unitOfWork)
    {
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _rolePermissionRepository = rolePermissionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Guid>> Handle(AssignRolePermissionsCommand request, CancellationToken ct)
    {
        Role role = await _roleRepository.GetByIdAsync(request.RoleId, ct) ??
            throw new NotFoundException($"Role with ID '{request.RoleId}' not found");

        IEnumerable<Permission> permissions = await _permissionRepository.FindAsync(p => request.PermissionIds.Contains(p.Id), ct);
        if (permissions.Count() != request.PermissionIds.Count())
        {
            throw new NotFoundException("One or more permissions not found");
        }

        IEnumerable<RolePermission> existingPermissions = await _rolePermissionRepository
            .FindAsync(rp => rp.RoleId == request.RoleId, ct);

        IEnumerable<RolePermission> permissionsToRemove = existingPermissions
            .Where(rp => !request.PermissionIds.Contains(rp.PermissionId));

        IEnumerable<RolePermission> permissionsToAdd = request.PermissionIds
            .Where(permissionId => !existingPermissions.Any(rp => rp.PermissionId == permissionId))
            .Select(permissionId => new RolePermission
            {
                RoleId = request.RoleId,
                PermissionId = permissionId
            });

        await _rolePermissionRepository.DeleteRangeAsync(permissionsToRemove, ct);
        await _rolePermissionRepository.AddRangeAsync(permissionsToAdd, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return request.PermissionIds;
    }
}