using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;


namespace MeUi.Application.Features.TenantAuthorization.Commands.AssignTenantUserRoles;

public class AssignTenantUserRolesCommandHandler : IRequestHandler<AssignTenantUserRolesCommand, IEnumerable<Guid>>
{
    private readonly IRepository<TenantUser> _tenantUserRepository;
    private readonly IRepository<TenantRole> _tenantRoleRepository;
    private readonly IRepository<TenantUserRole> _tenantUserRoleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignTenantUserRolesCommandHandler(
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

    public async Task<IEnumerable<Guid>> Handle(AssignTenantUserRolesCommand request, CancellationToken ct)
    {
        TenantUser user = await _tenantUserRepository.FirstOrDefaultAsync(tu => tu.Id == request.UserId && tu.TenantId == request.TenantId, ct) ??
            throw new NotFoundException($"User with ID '{request.UserId}' not found");

        IEnumerable<TenantRole> roles = await _tenantRoleRepository.FindAsync(r => request.RoleIds.Contains(r.Id), ct);
        if (roles.Count() != request.RoleIds.Count())
        {
            throw new NotFoundException("One or more roles not found");
        }

        IEnumerable<TenantUserRole> userRoles = await _tenantUserRoleRepository.FindAsync(ur => ur.TenantUserId == request.UserId, ct);

        IEnumerable<TenantUserRole> rolesToAdd = request.RoleIds
            .Where(roleId => !userRoles.Any(ur => ur.TenantRoleId == roleId))
            .Select(roleId => new TenantUserRole
            {
                TenantUserId = request.UserId,
                TenantRoleId = roleId
            });

        IEnumerable<TenantUserRole> rolesToRemove = userRoles.Where(ur => !request.RoleIds.Contains(ur.TenantRoleId));

        await _tenantUserRoleRepository.DeleteRangeAsync(rolesToRemove, ct);
        await _tenantUserRoleRepository.AddRangeAsync(rolesToAdd, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return request.RoleIds;
    }
}