using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;

namespace MeUi.Application.Features.TenantUsers.Commands.DeleteTenantUser;

public class DeleteTenantUserCommandHandler : IRequestHandler<DeleteTenantUserCommand, Unit>
{
    private readonly ITenantUserRepository _tenantUserRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTenantUserCommandHandler(
        ITenantUserRepository tenantUserRepository,
        IUnitOfWork unitOfWork)
    {
        _tenantUserRepository = tenantUserRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteTenantUserCommand request, CancellationToken cancellationToken)
    {
        var tenantUser = await _tenantUserRepository.GetByIdAsync(request.Id, cancellationToken);
        if (tenantUser == null)
        {
            throw new NotFoundException($"Tenant user with ID {request.Id} not found");
        }

        await _tenantUserRepository.DeleteAsync(tenantUser, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}