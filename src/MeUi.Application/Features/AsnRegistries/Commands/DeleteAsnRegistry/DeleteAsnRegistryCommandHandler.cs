using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.AsnRegistries.Commands.DeleteAsnRegistry;

public class DeleteAsnRegistryCommandHandler : IRequestHandler<DeleteAsnRegistryCommand, Guid>
{
    private readonly IRepository<AsnRegistry> _asnRegistryRepository;
    private readonly IRepository<TenantAsnRegistry> _tenantAsnRegistryRepository;
    private readonly IRepository<ThreatEvent> _threatEventRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteAsnRegistryCommandHandler(
        IRepository<AsnRegistry> asnRegistryRepository,
        IRepository<TenantAsnRegistry> tenantAsnRegistryRepository,
        IRepository<ThreatEvent> threatEventRepository,
        IUnitOfWork unitOfWork)
    {
        _asnRegistryRepository = asnRegistryRepository;
        _tenantAsnRegistryRepository = tenantAsnRegistryRepository;
        _threatEventRepository = threatEventRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(DeleteAsnRegistryCommand request, CancellationToken ct)
    {
        AsnRegistry asnRegistry = await _asnRegistryRepository.FirstOrDefaultAsync(
            asn => asn.Id == request.Id, ct) ??
            throw new NotFoundException($"ASN Registry with ID {request.Id} not found");

        bool hasTenantAssignments = await _tenantAsnRegistryRepository.ExistsAsync(
            tar => tar.AsnRegistryId == request.Id, ct);

        if (hasTenantAssignments)
        {
            throw new ConflictException("Cannot delete ASN Registry that is assigned to tenants. Please remove all tenant assignments first.");
        }

        bool hasThreatEvents = await _threatEventRepository.ExistsAsync(
            te => te.AsnRegistryId == request.Id, ct);

        if (hasThreatEvents)
        {
            throw new ConflictException("Cannot delete ASN Registry that has associated threat events. Please remove or reassign all threat events first.");
        }

        await _asnRegistryRepository.DeleteAsync(asnRegistry, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return asnRegistry.Id;
    }
}
