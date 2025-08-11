using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.TenantUsers.Commands.RemoveRoleFromTenantUser;

public class RemoveRoleFromTenantUserCommandHandler : IRequestHandler<RemoveRoleFromTenantUserCommand, Unit>
{
    private readonly IRepository<TenantUserRole> _tenantUserRoleRepository;
    private readonly IRepository<TenantRole> _tenantRoleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveRoleFromTenantUserCommandHandler(
        IRepository<TenantUserRole> tenantUserRoleRepository,
        IRepository<TenantRole> tenantRoleRepository,
        IUnitOfWork unitOfWork)
    {
        _tenantUserRoleRepository = tenantUserRoleRepository;
        _tenantRoleRepository = tenantRoleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(RemoveRoleFromTenantUserCommand request, CancellationToken ct)
    {
        IEnumerable<TenantUserRole> tenantUserRole = await _tenantUserRoleRepository
            .FindAsync(tur => tur.TenantUserId == request.TenantUserId && tur.TenantRoleId == request.RoleId, ct);

        TenantRole tenantRole = await _tenantRoleRepository.GetByIdAsync(request.RoleId, ct)
            ?? throw new NotFoundException("Role not found.");

        await _tenantUserRoleRepository.DeleteRangeAsync(tenantUserRole, ct);
        await _tenantRoleRepository.DeleteAsync(tenantRole, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Unit.Value;
    }
}