using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.TenantUsers.Commands.CreateTenantUser;

public class CreateTenantUserCommandHandler : IRequestHandler<CreateTenantUserCommand, Guid>
{
    private readonly ITenantUserRepository _tenantUserRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTenantUserCommandHandler(
        ITenantUserRepository tenantUserRepository,
        ITenantRepository tenantRepository,
        IUnitOfWork unitOfWork)
    {
        _tenantUserRepository = tenantUserRepository;
        _tenantRepository = tenantRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateTenantUserCommand request, CancellationToken cancellationToken)
    {
        // Validate tenant exists
        var tenant = await _tenantRepository.GetByIdAsync(request.TenantId, cancellationToken);
        if (tenant == null)
        {
            throw new NotFoundException($"Tenant with ID {request.TenantId} not found");
        }

        // Check if username is unique within tenant
        var isUsernameUnique = await _tenantUserRepository.IsUsernameUniqueInTenantAsync(
            request.Username, request.TenantId, null, cancellationToken);
        if (!isUsernameUnique)
        {
            throw new ConflictException($"Username '{request.Username}' already exists in this tenant");
        }

        // Check if email is unique within tenant
        var isEmailUnique = await _tenantUserRepository.IsEmailUniqueInTenantAsync(
            request.Email, request.TenantId, null, cancellationToken);
        if (!isEmailUnique)
        {
            throw new ConflictException($"Email '{request.Email}' already exists in this tenant");
        }

        var tenantUser = new TenantUser
        {
            Username = request.Username,
            Email = request.Email,
            Name = request.Name,
            TenantId = request.TenantId,
            IsTenantAdmin = request.IsTenantAdmin,
            IsSuspended = false
        };

        await _tenantUserRepository.AddAsync(tenantUser, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Assign roles if provided
        foreach (var roleId in request.RoleIds)
        {
            await _tenantUserRepository.AssignRoleToUserAsync(tenantUser.Id, roleId, cancellationToken);
        }

        if (request.RoleIds.Any())
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return tenantUser.Id;
    }
}