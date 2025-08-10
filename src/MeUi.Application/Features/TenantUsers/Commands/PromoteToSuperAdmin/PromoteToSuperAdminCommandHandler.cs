using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;

namespace MeUi.Application.Features.TenantUsers.Commands.PromoteToSuperAdmin;

public class PromoteToSuperAdminCommandHandler : IRequestHandler<PromoteToSuperAdminCommand, Unit>
{
    private readonly ITenantUserRepository _tenantUserRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PromoteToSuperAdminCommandHandler(
        ITenantUserRepository tenantUserRepository,
        IUnitOfWork unitOfWork)
    {
        _tenantUserRepository = tenantUserRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(PromoteToSuperAdminCommand request, CancellationToken cancellationToken)
    {
        var tenantUser = await _tenantUserRepository.GetByIdAsync(request.TenantUserId, cancellationToken);
        if (tenantUser == null)
        {
            throw new NotFoundException($"Tenant user with ID {request.TenantUserId} not found");
        }

        if (tenantUser.IsTenantAdmin)
        {
            throw new ConflictException("User is already a super admin");
        }

        await _tenantUserRepository.PromoteToSuperAdminAsync(request.TenantUserId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}