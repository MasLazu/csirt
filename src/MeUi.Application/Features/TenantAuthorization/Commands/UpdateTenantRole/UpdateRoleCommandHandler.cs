using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;


namespace MeUi.Application.Features.TenantAuthorization.Commands.UpdateTenantRole;

public class UpdateTenantRoleCommandHandler : IRequestHandler<UpdateTenantRoleCommand, Guid>
{
    private readonly IRepository<TenantRole> _tenantRoleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTenantRoleCommandHandler(
        IRepository<TenantRole> tenantRoleRepository,
        IUnitOfWork unitOfWork)
    {
        _tenantRoleRepository = tenantRoleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(UpdateTenantRoleCommand request, CancellationToken ct)
    {
        TenantRole role = await _tenantRoleRepository.FirstOrDefaultAsync(rt => rt.Id == request.Id && rt.TenantId == request.TenantId, ct) ??
            throw new InvalidOperationException($"Role with ID '{request.Id}' not found");

        role.Name = request.Name;
        role.Description = request.Description;

        await _tenantRoleRepository.UpdateAsync(role, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return role.Id;
    }
}