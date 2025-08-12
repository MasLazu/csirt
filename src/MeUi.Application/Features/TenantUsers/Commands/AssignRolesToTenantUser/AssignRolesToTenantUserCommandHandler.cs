using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.TenantUsers.Commands.AssignRolesToTenantUser;

public class AssignRolesToTenantUserCommandHandler : IRequestHandler<AssignRolesToTenantUserCommand, IEnumerable<Guid>>
{
    private readonly IRepository<TenantUser> _tenantUserRepository;
    private readonly IRepository<TenantRole> _tenantRoleRepository;
    private readonly IRepository<TenantUserRole> _tenantUserRoleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignRolesToTenantUserCommandHandler(
        IRepository<TenantUser> tenantUserRepository,
        IRepository<TenantRole> tenantRoleRepository,
        IRepository<TenantUserRole> tenantUserRoleRepository,
        IUnitOfWork unitOfWork)
    {
        _tenantUserRepository = tenantUserRepository;
        _tenantRoleRepository = tenantRoleRepository;
        _tenantUserRoleRepository = tenantUserRoleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Guid>> Handle(AssignRolesToTenantUserCommand request, CancellationToken ct)
    {
        // Validate tenant user exists and belongs to the specified tenant
        TenantUser tenantUser = await _tenantUserRepository.FirstOrDefaultAsync(
            tu => tu.Id == request.UserId && tu.TenantId == request.TenantId, ct) ??
            throw new NotFoundException($"Tenant user with ID {request.UserId} not found in tenant {request.TenantId}");

        // Validate all roles exist and belong to the tenant
        IEnumerable<TenantRole> roles = await _tenantRoleRepository.FindAsync(
            tr => request.RoleIds.Contains(tr.Id) && tr.TenantId == request.TenantId, ct);

        if (roles.Count() != request.RoleIds.Count())
        {
            throw new NotFoundException("One or more roles not found in the specified tenant");
        }

        // Get existing user roles
        IEnumerable<TenantUserRole> existingUserRoles = await _tenantUserRoleRepository.FindAsync(
            tur => tur.TenantUserId == request.UserId, ct);

        // Remove roles not in the new list
        IEnumerable<TenantUserRole> rolesToRemove = existingUserRoles
            .Where(tur => !request.RoleIds.Contains(tur.TenantRoleId));

        // Add new roles
        IEnumerable<TenantUserRole> rolesToAdd = request.RoleIds
            .Where(roleId => !existingUserRoles.Any(tur => tur.TenantRoleId == roleId))
            .Select(roleId => new TenantUserRole
            {
                TenantUserId = request.UserId,
                TenantRoleId = roleId
            });

        await _tenantUserRoleRepository.DeleteRangeAsync(rolesToRemove, ct);
        await _tenantUserRoleRepository.AddRangeAsync(rolesToAdd, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return request.RoleIds;
    }
}
