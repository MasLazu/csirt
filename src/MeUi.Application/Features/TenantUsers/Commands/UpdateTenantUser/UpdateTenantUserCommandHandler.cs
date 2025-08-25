using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.TenantUsers.Commands.UpdateTenantUser;

public class UpdateTenantUserCommandHandler : IRequestHandler<UpdateTenantUserCommand, Unit>
{
    private readonly IRepository<TenantUser> _tenantUserRepository;
    private readonly IRepository<TenantRole> _tenantRoleRepository;
    private readonly IRepository<TenantUserRole> _tenantUserRoleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTenantUserCommandHandler(
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

    public async Task<Unit> Handle(UpdateTenantUserCommand request, CancellationToken cancellationToken)
    {
        TenantUser tenantUser = await _tenantUserRepository.FirstOrDefaultAsync(
            tu => tu.Id == request.UserId && tu.TenantId == request.TenantId,
            cancellationToken)
            ?? throw new NotFoundException("Tenant user not found.");

        tenantUser.Email = request.Email;
        tenantUser.Username = request.Username;
        tenantUser.Name = request.Name;

        if (request.RoleIds != null && request.RoleIds.Any())
        {
            if (await _tenantRoleRepository.CountAsync(tr => request.RoleIds.Contains(tr.Id) && tr.TenantId == request.TenantId, cancellationToken) != request.RoleIds.Count())
            {
                throw new BadRequestException("One or more roles do not exist in the specified tenant.");
            }
        }

        await _tenantUserRepository.UpdateAsync(tenantUser, cancellationToken);

        IEnumerable<TenantUserRole> existingRoles = await _tenantUserRoleRepository.FindAsync(tur => tur.TenantUserId == request.UserId, cancellationToken);

        foreach (TenantUserRole er in existingRoles)
        {
            await _tenantUserRoleRepository.DeleteAsync(er, cancellationToken);
        }

        if (request.RoleIds != null && request.RoleIds.Any())
        {
            foreach (Guid roleId in request.RoleIds)
            {
                var tur = new TenantUserRole
                {
                    TenantUserId = request.UserId,
                    TenantRoleId = roleId
                };
                await _tenantUserRoleRepository.AddAsync(tur, cancellationToken);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}