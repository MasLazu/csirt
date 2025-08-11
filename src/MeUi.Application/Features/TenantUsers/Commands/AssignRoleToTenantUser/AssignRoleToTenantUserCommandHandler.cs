using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.TenantUsers.Commands.AssignRoleToTenantUser;

public class AssignRoleToTenantUserCommandHandler : IRequestHandler<AssignRoleToTenantUserCommand, Unit>
{
    private readonly IRepository<TenantUser> _tenantUserRepository;
    private readonly IRepository<TenantRole> _tenantRoleRepository;
    private readonly IRepository<TenantUserRole> _tenantUserRoleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignRoleToTenantUserCommandHandler(
        IRepository<TenantUser> tenantUserRepository,
        IRepository<TenantRole> roleRepository,
        IRepository<TenantUserRole> tenantUserRoleRepository,
        IUnitOfWork unitOfWork)
    {
        _tenantUserRepository = tenantUserRepository;
        _tenantRoleRepository = roleRepository;
        _tenantUserRoleRepository = tenantUserRoleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(AssignRoleToTenantUserCommand request, CancellationToken ct)
    {
        if (!await _tenantRoleRepository.ExistsAsync(request.RoleId, ct))
        {
            throw new NotFoundException("Role not found.");
        }

        if (!await _tenantUserRepository.ExistsAsync(request.TenantUserId, ct))
        {
            throw new NotFoundException("Tenant user not found.");
        }

        if (await _tenantUserRoleRepository.ExistsAsync(tur => tur.TenantUserId == request.TenantUserId && tur.TenantRoleId == request.RoleId, ct))
        {
            return Unit.Value;
        }

        var tenantUserRole = new TenantUserRole
        {
            TenantUserId = request.TenantUserId,
            TenantRoleId = request.RoleId
        };

        await _tenantUserRoleRepository.AddAsync(tenantUserRole, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Unit.Value;
    }
}