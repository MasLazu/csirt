using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.Tenants.Commands.UpdateTenant;

public class UpdateTenantCommandHandler : IRequestHandler<UpdateTenantCommand, Unit>
{
    private readonly IRepository<Tenant> _tenantRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTenantCommandHandler(
        IRepository<Tenant> tenantRepository,
        IUnitOfWork unitOfWork)
    {
        _tenantRepository = tenantRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateTenantCommand request, CancellationToken ct)
    {
        Tenant tenant = await _tenantRepository.GetByIdAsync(request.Id, ct) ??
            throw new NotFoundException($"Tenant with ID {request.Id} not found");

        tenant.Name = request.Name;
        tenant.Description = request.Description;
        tenant.ContactEmail = request.ContactEmail;
        tenant.ContactPhone = request.ContactPhone;
        tenant.IsActive = request.IsActive;
        tenant.UpdatedAt = DateTime.UtcNow;

        await _tenantRepository.UpdateAsync(tenant, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Unit.Value;
    }
}