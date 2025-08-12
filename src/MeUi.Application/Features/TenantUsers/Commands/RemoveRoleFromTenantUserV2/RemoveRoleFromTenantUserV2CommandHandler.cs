using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.TenantUsers.Commands.RemoveRoleFromTenantUserV2;

public class RemoveRoleFromTenantUserV2CommandHandler : IRequestHandler<RemoveRoleFromTenantUserV2Command, Unit>
{
    private readonly IRepository<TenantUser> _tenantUserRepository;
    private readonly IRepository<TenantRole> _tenantRoleRepository;
    private readonly IRepository<TenantUserRole> _tenantUserRoleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveRoleFromTenantUserV2CommandHandler(
        IRepository<TenantUser> tenantUserRepository,
        IRepository<TenantRole> tenantRoleRepository,
        IRepository<TenantUserRole> tenantUserRoleRepository,
        IUnitOfWork unitOfWork)
    {
        _tenantUserRepository = tenantUserRepository;
        _tenantRoleRepository = tenantRoleRepository;
        _tenantUserRoleRepository = tenantUserRoleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(RemoveRoleFromTenantUserV2Command request, CancellationToken ct)
    {
        TenantUser tenantUser = await _tenantUserRepository.FirstOrDefaultAsync(
            tu => tu.Id == request.UserId && tu.TenantId == request.TenantId, ct) ??
            throw new NotFoundException($"Tenant user with ID {request.UserId} not found in tenant {request.TenantId}");

        TenantRole tenantRole = await _tenantRoleRepository.FirstOrDefaultAsync(
            tr => tr.Id == request.RoleId && tr.TenantId == request.TenantId, ct) ??
            throw new NotFoundException($"Role with ID {request.RoleId} not found in tenant {request.TenantId}");

        TenantUserRole tenantUserRole = await _tenantUserRoleRepository.FirstOrDefaultAsync(
            tur => tur.TenantUserId == request.UserId && tur.TenantRoleId == request.RoleId, ct) ??
            throw new NotFoundException("Role assignment not found for this user");

        await _tenantUserRoleRepository.DeleteAsync(tenantUserRole, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
