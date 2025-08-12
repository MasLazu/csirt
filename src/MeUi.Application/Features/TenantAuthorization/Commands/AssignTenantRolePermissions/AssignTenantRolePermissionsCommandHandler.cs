using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;


namespace MeUi.Application.Features.TenantAuthorization.Commands.AssignTenantRolePermissions;

public class AssignTenantRolePermissionsCommandHandler : IRequestHandler<AssignTenantRolePermissionsCommand, IEnumerable<Guid>>
{
    private readonly IRepository<TenantRole> _tenantRoleRepository;
    private readonly IRepository<TenantPermission> _tenantPermissionRepository;
    private readonly IRepository<TenantRolePermission> _tenantRolePermissionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignTenantRolePermissionsCommandHandler(
        IRepository<TenantRole> tenantRoleRepository,
        IRepository<TenantPermission> tenantPermissionRepository,
        IRepository<TenantRolePermission> tenantRolePermissionRepository,
        IUnitOfWork unitOfWork)
    {
        _tenantRoleRepository = tenantRoleRepository;
        _tenantPermissionRepository = tenantPermissionRepository;
        _tenantRolePermissionRepository = tenantRolePermissionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Guid>> Handle(AssignTenantRolePermissionsCommand request, CancellationToken ct)
    {
        TenantRole role = await _tenantRoleRepository.FirstOrDefaultAsync(tr => tr.TenantId == request.TenantId && tr.Id == request.RoleId, ct) ??
            throw new NotFoundException($"Role with ID '{request.RoleId}' not found");

        IEnumerable<Guid> permissions = await _tenantPermissionRepository.FindAsync(tp => request.PermissionIds.Contains(tp.Id), tp => tp.Id, ct);
        if (permissions.Count() != request.PermissionIds.Count())
        {
            throw new NotFoundException("One or more permissions not found");
        }

        IEnumerable<TenantRolePermission> existingPermissions = await _tenantRolePermissionRepository
            .FindAsync(rp => rp.TenantRoleId == request.RoleId, ct);

        IEnumerable<TenantRolePermission> permissionsToRemove = existingPermissions
            .Where(rp => !request.PermissionIds.Contains(rp.TenantPermissionId));

        IEnumerable<TenantRolePermission> permissionsToAdd = request.PermissionIds
            .Where(permissionId => !existingPermissions.Any(rp => rp.TenantPermissionId == permissionId))
            .Select(permissionId => new TenantRolePermission
            {
                TenantRoleId = request.RoleId,
                TenantPermissionId = permissionId
            });

        await _tenantRolePermissionRepository.DeleteRangeAsync(permissionsToRemove, ct);
        await _tenantRolePermissionRepository.AddRangeAsync(permissionsToAdd, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return request.PermissionIds;
    }
}