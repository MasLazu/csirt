using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.Tenants.Commands.BulkAssignAsnRegistriesToTenant;

public class BulkAssignAsnRegistriesToTenantCommandHandler : IRequestHandler<BulkAssignAsnRegistriesToTenantCommand, Unit>
{
    private readonly IRepository<Tenant> _tenantRepository;
    private readonly IRepository<AsnRegistry> _asnRepository;
    private readonly IRepository<TenantAsnRegistry> _tenantAsnRegistryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public BulkAssignAsnRegistriesToTenantCommandHandler(
        IUnitOfWork unitOfWork,
        IRepository<Tenant> tenantRepository,
        IRepository<AsnRegistry> asnRepository,
        IRepository<TenantAsnRegistry> tenantAsnRegistryRepository)
    {
        _tenantRepository = tenantRepository;
        _asnRepository = asnRepository;
        _tenantAsnRegistryRepository = tenantAsnRegistryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(BulkAssignAsnRegistriesToTenantCommand request, CancellationToken ct)
    {
        if (!await _tenantRepository.ExistsAsync(request.TenantId, ct))
        {
            throw new NotFoundException($"Tenant with ID {request.TenantId} not found");
        }

        int asnCount = await _asnRepository.CountAsync(ar => request.AsnRegistryIds.Contains(ar.Id), ct);

        if (asnCount != request.AsnRegistryIds.Count)
        {
            throw new NotFoundException("One or more ASN Registries not found");
        }

        var tenantAsnRegistries = request.AsnRegistryIds.Select(asnId => new TenantAsnRegistry
        {
            TenantId = request.TenantId,
            AsnRegistryId = asnId
        }).ToList();

        await _tenantAsnRegistryRepository.AddRangeAsync(tenantAsnRegistries, ct);

        await _unitOfWork.SaveChangesAsync(ct);

        return Unit.Value;
    }
}