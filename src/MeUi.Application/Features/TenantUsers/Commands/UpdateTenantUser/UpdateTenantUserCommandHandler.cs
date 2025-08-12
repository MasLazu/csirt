using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.TenantUsers.Commands.UpdateTenantUser;

public class UpdateTenantUserCommandHandler : IRequestHandler<UpdateTenantUserCommand, Unit>
{
    private readonly IRepository<TenantUser> _tenantUserRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTenantUserCommandHandler(
        IRepository<TenantUser> tenantUserRepository,
        IUnitOfWork unitOfWork)
    {
        _tenantUserRepository = tenantUserRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateTenantUserCommand request, CancellationToken cancellationToken)
    {
        // Find the tenant user by user ID and tenant ID
        TenantUser tenantUser = await _tenantUserRepository.FirstOrDefaultAsync(
            tu => tu.Id == request.UserId && tu.TenantId == request.TenantId,
            cancellationToken)
            ?? throw new NotFoundException("Tenant user not found.");

        tenantUser.Email = request.Email;
        tenantUser.Username = request.Username;
        tenantUser.Name = request.Name;

        await _tenantUserRepository.UpdateAsync(tenantUser, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}