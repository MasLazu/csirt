using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.Tenants.Commands.AssignAsnRegistriesToTenant;

public class AssignAsnRegistryToTenantCommandHandler : IRequestHandler<AssignAsnRegistryToTenantCommand, Unit>
{
    private readonly IRepository<Tenant> _tenantRepository;
    private readonly IRepository<AsnRegistry> _asnRepository;
    private readonly IRepository<TenantAsnRegistry> _tenantAsnRegistryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignAsnRegistryToTenantCommandHandler(
        IRepository<Tenant> tenantRepository,
        IRepository<AsnRegistry> asnRepository,
        IRepository<TenantAsnRegistry> tenantAsnRegistryRepository,
        IUnitOfWork unitOfWork)
    {
        _tenantRepository = tenantRepository;
        _asnRepository = asnRepository;
        _tenantAsnRegistryRepository = tenantAsnRegistryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(AssignAsnRegistryToTenantCommand request, CancellationToken ct)
    {
        if (!await _tenantRepository.ExistsAsync(request.TenantId, ct))
        {
            throw new NotFoundException($"Tenant with ID {request.TenantId} not found");
        }

        if (!await _asnRepository.ExistsAsync(request.AsnId, ct))
        {
            throw new NotFoundException($"ASN Registry with ID {request.AsnId} not found");
        }

        TenantAsnRegistry? tenantAsnRegistry = await _tenantAsnRegistryRepository
            .FirstOrDefaultAsync(tar => tar.TenantId == request.TenantId && tar.AsnRegistryId == request.AsnId, ct);

        if (tenantAsnRegistry == null)
        {
            tenantAsnRegistry = new TenantAsnRegistry
            {
                TenantId = request.TenantId,
                AsnRegistryId = request.AsnId
            };
            await _tenantAsnRegistryRepository.AddAsync(tenantAsnRegistry, ct);
        }

        await _unitOfWork.SaveChangesAsync(ct);

        return Unit.Value;
    }
}