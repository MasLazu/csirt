using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.TenantUsers.Commands.AssignRoleToTenantUser;

public class AssignRoleToTenantUserCommandHandler : IRequestHandler<AssignRoleToTenantUserCommand, Unit>
{
    private readonly IRepository<TenantUser> _tenantUserRepository;
    private readonly IRepository<Role> _roleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignRoleToTenantUserCommandHandler(
        ITenantUserRepository tenantUserRepository,
        IRepository<MeUi.Domain.Entities.Role> roleRepository,
        IUnitOfWork unitOfWork)
    {
        _tenantUserRepository = tenantUserRepository;
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(AssignRoleToTenantUserCommand request, CancellationToken cancellationToken)
    {
        // Validate tenant user exists
        var tenantUser = await _tenantUserRepository.GetByIdAsync(request.TenantUserId, cancellationToken);
        if (tenantUser == null)
        {
            throw new NotFoundException($"Tenant user with ID {request.TenantUserId} not found");
        }

        // Validate role exists
        var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
        if (role == null)
        {
            throw new NotFoundException($"Role with ID {request.RoleId} not found");
        }

        // Check if user already has this role
        var hasRole = await _tenantUserRepository.UserHasRoleAsync(request.TenantUserId, request.RoleId, cancellationToken);
        if (hasRole)
        {
            throw new ConflictException($"User already has the role '{role.Name}'");
        }

        await _tenantUserRepository.AssignRoleToUserAsync(request.TenantUserId, request.RoleId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}