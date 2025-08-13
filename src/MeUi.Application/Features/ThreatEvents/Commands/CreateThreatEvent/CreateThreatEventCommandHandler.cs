using Mapster;
using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.ThreatEvents.Commands.CreateThreatEvent;

public class CreateThreatEventCommandHandler : IRequestHandler<CreateThreatEventCommand, Guid>
{
    private readonly IRepository<ThreatEvent> _threatEventRepository;
    private readonly IRepository<AsnRegistry> _asnRegistryRepository;
    private readonly IRepository<Country> _countryRepository;
    private readonly IRepository<Protocol> _protocolRepository;
    private readonly IRepository<MalwareFamily> _malwareFamilyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateThreatEventCommandHandler(
        IRepository<ThreatEvent> threatEventRepository,
        IRepository<AsnRegistry> asnRegistryRepository,
        IRepository<Country> countryRepository,
        IRepository<Protocol> protocolRepository,
        IRepository<MalwareFamily> malwareFamilyRepository,
        IUnitOfWork unitOfWork)
    {
        _threatEventRepository = threatEventRepository;
        _asnRegistryRepository = asnRegistryRepository;
        _countryRepository = countryRepository;
        _protocolRepository = protocolRepository;
        _malwareFamilyRepository = malwareFamilyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateThreatEventCommand request, CancellationToken ct)
    {
        if (!await _asnRegistryRepository.ExistsAsync(request.AsnRegistryId, ct))
        {
            throw new NotFoundException($"ASN Registry with ID {request.AsnRegistryId} not found");
        }

        if (request.SourceCountryId.HasValue && !await _countryRepository.ExistsAsync(request.SourceCountryId.Value, ct))
        {
            throw new NotFoundException($"Source Country with ID {request.SourceCountryId} not found");
        }

        if (request.DestinationCountryId.HasValue && !await _countryRepository.ExistsAsync(request.DestinationCountryId.Value, ct))
        {
            throw new NotFoundException($"Destination Country with ID {request.DestinationCountryId} not found");
        }

        if (request.ProtocolId.HasValue && !await _protocolRepository.ExistsAsync(request.ProtocolId.Value, ct))
        {
            throw new NotFoundException($"Protocol with ID {request.ProtocolId} not found");
        }

        if (request.MalwareFamilyId.HasValue && !await _malwareFamilyRepository.ExistsAsync(request.MalwareFamilyId.Value, ct))
        {
            throw new NotFoundException($"Malware Family with ID {request.MalwareFamilyId} not found");
        }

        ThreatEvent threatEvent = request.Adapt<ThreatEvent>();
        threatEvent.Id = Guid.NewGuid();
        threatEvent.CreatedAt = DateTime.UtcNow;

        await _threatEventRepository.AddAsync(threatEvent, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return threatEvent.Id;
    }
}
