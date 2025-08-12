using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;


namespace MeUi.Application.Features.TenantAuthorization.Commands.DeleteTenantRole;

public class DeleteTenantRoleCommandHandler : IRequestHandler<DeleteTenantRoleCommand, Guid>
{
    private readonly IRepository<TenantRole> _tenantRoleRepository;
    private readonly IRepository<TenantUserRole> _tenantUserRoleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTenantRoleCommandHandler(
        IRepository<TenantRole> tenantRoleRepository,
        IRepository<TenantUserRole> tenantUserRoleRepository,
        IUnitOfWork unitOfWork)
    {
        _tenantRoleRepository = tenantRoleRepository;
        _tenantUserRoleRepository = tenantUserRoleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(DeleteTenantRoleCommand request, CancellationToken ct)
    {
        TenantRole role = await _tenantRoleRepository.FirstOrDefaultAsync(tr => tr.Id == request.Id && tr.TenantId == request.TenantId, ct) ??
            throw new InvalidOperationException($"Role with ID '{request.Id}' not found");

        IEnumerable<TenantUserRole> userRoles = await _tenantUserRoleRepository.FindAsync(ur => ur.TenantRoleId == role.Id, ct) ??
            throw new InvalidOperationException("Cannot delete role that is assigned to users");

        await _tenantRoleRepository.DeleteAsync(role, ct);
        await _tenantUserRoleRepository.DeleteRangeAsync(userRoles, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return role.Id;
    }
}