using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;

namespace MeUi.Application.Features.TenantUsers.Commands.DemoteFromSuperAdmin;

public class DemoteFromSuperAdminCommandHandler : IRequestHandler<DemoteFromSuperAdminCommand, Unit>
{
    private readonly ITenantUserRepository _tenantUserRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DemoteFromSuperAdminCommandHandler(
        ITenantUserRepository tenantUserRepository,
        IUnitOfWork unitOfWork)
    {
        _tenantUserRepository = tenantUserRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DemoteFromSuperAdminCommand request, CancellationToken cancellationToken)
    {
        var tenantUser = await _tenantUserRepository.GetByIdAsync(request.TenantUserId, cancellationToken);
        if (tenantUser == null)
        {
            throw new NotFoundException($"Tenant user with ID {request.TenantUserId} not found");
        }

        if (!tenantUser.IsTenantAdmin)
        {
            throw new ConflictException("User is not a super admin");
        }

        await _tenantUserRepository.DemoteFromSuperAdminAsync(request.TenantUserId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}