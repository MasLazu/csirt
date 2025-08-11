using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.TenantUsers.Commands.DeleteTenantUser;

public class DeleteTenantUserCommandHandler : IRequestHandler<DeleteTenantUserCommand, Unit>
{
    private readonly IRepository<TenantUser> _tenantUserRepository;
    private readonly IRepository<TenantUserRole> _tenantUserRoleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTenantUserCommandHandler(
        IRepository<TenantUser> tenantUserRepository,
        IRepository<TenantUserRole> tenantUserRoleRepository,
        IUnitOfWork unitOfWork)
    {
        _tenantUserRepository = tenantUserRepository;
        _tenantUserRoleRepository = tenantUserRoleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteTenantUserCommand request, CancellationToken cancellationToken)
    {
        IEnumerable<TenantUserRole> tenantUserRole = await _tenantUserRoleRepository.FindAsync(tur => tur.TenantUserId == request.Id, cancellationToken);
        TenantUser tenantUser = await _tenantUserRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Tenant user not found.");

        await _tenantUserRoleRepository.DeleteRangeAsync(tenantUserRole, cancellationToken);
        await _tenantUserRepository.DeleteAsync(tenantUser, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}