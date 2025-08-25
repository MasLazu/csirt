using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;


namespace MeUi.Application.Features.TenantAuthorization.Commands.UpdateTenantRole;

public class UpdateTenantRoleCommandHandler : IRequestHandler<UpdateTenantRoleCommand, Guid>
{
    private readonly IRepository<TenantRole> _tenantRoleRepository;
    private readonly IRepository<TenantRolePermission> _tenantRolePermissionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTenantRoleCommandHandler(
    IRepository<TenantRole> tenantRoleRepository,
    IRepository<TenantRolePermission> tenantRolePermissionRepository,
        IUnitOfWork unitOfWork)
    {
        _tenantRoleRepository = tenantRoleRepository;
        _tenantRolePermissionRepository = tenantRolePermissionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(UpdateTenantRoleCommand request, CancellationToken ct)
    {
        TenantRole role = await _tenantRoleRepository.FirstOrDefaultAsync(rt => rt.Id == request.Id && rt.TenantId == request.TenantId, ct) ??
            throw new InvalidOperationException($"Role with ID '{request.Id}' not found");

        role.Name = request.Name;
        role.Description = request.Description;

        await _tenantRoleRepository.UpdateAsync(role, ct);

        IEnumerable<TenantRolePermission> existingRolePermissions = await _tenantRolePermissionRepository.FindAsync(rp => rp.TenantRoleId == request.Id, ct);
        var existingPermissionIds = existingRolePermissions.Select(rp => rp.TenantPermissionId).ToHashSet();
        var requestedPermissionIds = request.PermissionIds.ToHashSet();

        IEnumerable<TenantRolePermission> permissionsToRemove = existingRolePermissions.Where(rp => !requestedPermissionIds.Contains(rp.TenantPermissionId));

        await _tenantRolePermissionRepository.DeleteRangeAsync(permissionsToRemove, ct);

        IEnumerable<Guid> permissionIdsToAdd = requestedPermissionIds.Where(pid => !existingPermissionIds.Contains(pid));
        foreach (Guid permissionId in permissionIdsToAdd)
        {
            var rolePermission = new TenantRolePermission
            {
                TenantRoleId = request.Id,
                TenantPermissionId = permissionId
            };
            await _tenantRolePermissionRepository.AddAsync(rolePermission, ct);
        }

        await _unitOfWork.SaveChangesAsync(ct);

        return role.Id;
    }
}