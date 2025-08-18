using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;


namespace MeUi.Application.Features.Authorization.Commands.UpdateRole;

public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, Guid>
{
    private readonly IRepository<Role> _roleRepository;
    private readonly IRepository<RolePermission> _rolePermissionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateRoleCommandHandler(
        IRepository<Role> roleRepository,
        IRepository<RolePermission> rolePermissionRepository,
        IUnitOfWork unitOfWork)
    {
        _roleRepository = roleRepository;
        _rolePermissionRepository = rolePermissionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(UpdateRoleCommand request, CancellationToken ct)
    {
        Role role = await _roleRepository.GetByIdAsync(request.Id, ct) ??
            throw new InvalidOperationException($"Role with ID '{request.Id}' not found");

        role.Name = request.Name;
        role.Description = request.Description;

        await _roleRepository.UpdateAsync(role, ct);

        IEnumerable<RolePermission> existingRolePermissions = await _rolePermissionRepository.FindAsync(rp => rp.RoleId == request.Id, ct);
        var existingPermissionIds = existingRolePermissions.Select(rp => rp.PermissionId).ToHashSet();
        var requestedPermissionIds = request.PermissionIds.ToHashSet();

        IEnumerable<RolePermission> permissionsToRemove = existingRolePermissions.Where(rp => !requestedPermissionIds.Contains(rp.PermissionId));
        foreach (RolePermission permissionToRemove in permissionsToRemove)
        {
            await _rolePermissionRepository.DeleteAsync(permissionToRemove, ct);
        }

        IEnumerable<Guid> permissionIdsToAdd = requestedPermissionIds.Where(pid => !existingPermissionIds.Contains(pid));
        foreach (Guid permissionId in permissionIdsToAdd)
        {
            var rolePermission = new RolePermission
            {
                RoleId = request.Id,
                PermissionId = permissionId
            };
            await _rolePermissionRepository.AddAsync(rolePermission, ct);
        }

        await _unitOfWork.SaveChangesAsync(ct);

        return role.Id;
    }
}