using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;

namespace MeUi.Application.Features.TenantUsers.Commands.RemoveRoleFromTenantUser;

public class RemoveRoleFromTenantUserCommandHandler : IRequestHandler<RemoveRoleFromTenantUserCommand, Unit>
{
    private readonly ITenantUserRepository _tenantUserRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveRoleFromTenantUserCommandHandler(
        ITenantUserRepository tenantUserRepository,
        IUnitOfWork unitOfWork)
    {
        _tenantUserRepository = tenantUserRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(RemoveRoleFromTenantUserCommand request, CancellationToken cancellationToken)
    {
        // Validate tenant user exists
        var tenantUser = await _tenantUserRepository.GetByIdAsync(request.TenantUserId, cancellationToken);
        if (tenantUser == null)
        {
            throw new NotFoundException($"Tenant user with ID {request.TenantUserId} not found");
        }

        // Check if user has this role
        var hasRole = await _tenantUserRepository.UserHasRoleAsync(request.TenantUserId, request.RoleId, cancellationToken);
        if (!hasRole)
        {
            throw new NotFoundException($"User does not have the specified role");
        }

        await _tenantUserRepository.RemoveRoleFromUserAsync(request.TenantUserId, request.RoleId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}