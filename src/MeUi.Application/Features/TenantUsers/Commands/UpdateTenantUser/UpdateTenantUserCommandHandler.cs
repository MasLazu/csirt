using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;

namespace MeUi.Application.Features.TenantUsers.Commands.UpdateTenantUser;

public class UpdateTenantUserCommandHandler : IRequestHandler<UpdateTenantUserCommand, Unit>
{
    private readonly ITenantUserRepository _tenantUserRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTenantUserCommandHandler(
        ITenantUserRepository tenantUserRepository,
        IUnitOfWork unitOfWork)
    {
        _tenantUserRepository = tenantUserRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateTenantUserCommand request, CancellationToken cancellationToken)
    {
        var tenantUser = await _tenantUserRepository.GetByIdAsync(request.Id, cancellationToken);
        if (tenantUser == null)
        {
            throw new NotFoundException($"Tenant user with ID {request.Id} not found");
        }

        // Check if username is unique within tenant (excluding current user)
        var isUsernameUnique = await _tenantUserRepository.IsUsernameUniqueInTenantAsync(
            request.Username, tenantUser.TenantId, request.Id, cancellationToken);
        if (!isUsernameUnique)
        {
            throw new ConflictException($"Username '{request.Username}' already exists in this tenant");
        }

        // Check if email is unique within tenant (excluding current user)
        var isEmailUnique = await _tenantUserRepository.IsEmailUniqueInTenantAsync(
            request.Email, tenantUser.TenantId, request.Id, cancellationToken);
        if (!isEmailUnique)
        {
            throw new ConflictException($"Email '{request.Email}' already exists in this tenant");
        }

        tenantUser.Username = request.Username;
        tenantUser.Email = request.Email;
        tenantUser.Name = request.Name;
        tenantUser.IsSuspended = request.IsSuspended;
        tenantUser.IsTenantAdmin = request.IsTenantAdmin;

        await _tenantUserRepository.UpdateAsync(tenantUser, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}