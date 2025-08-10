using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.Tenants.Commands.RemoveAsnFromTenant;

public class RemoveAsnFromTenantCommandHandler : IRequestHandler<RemoveAsnFromTenantCommand, Unit>
{
    private readonly IRepository<TenantAsnRegistry> _tenantAsnRegistryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveAsnFromTenantCommandHandler(
        IRepository<TenantAsnRegistry> tenantAsnRegistryRepository,
        IUnitOfWork unitOfWork)
    {
        _tenantAsnRegistryRepository = tenantAsnRegistryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(RemoveAsnFromTenantCommand request, CancellationToken ct)
    {
        TenantAsnRegistry tenantAsnRegistry = await _tenantAsnRegistryRepository
            .FirstOrDefaultAsync(tar => tar.TenantId == request.TenantId && tar.AsnRegistryId == request.AsnId, ct) ??
            throw new NotFoundException($"ASN Registry with ID {request.AsnId} not found for Tenant with ID {request.TenantId}");

        await _tenantAsnRegistryRepository.DeleteAsync(tenantAsnRegistry, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Unit.Value;
    }
}