using Mapster;
using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.ThreatEvents.Commands.UpdateThreatEvent;

public class UpdateThreatEventCommandHandler : IRequestHandler<UpdateThreatEventCommand>
{
    private readonly IRepository<ThreatEvent> _threatEventRepository;
    private readonly IRepository<AsnRegistry> _asnRegistryRepository;
    private readonly IRepository<Country> _countryRepository;
    private readonly IRepository<Protocol> _protocolRepository;
    private readonly IRepository<MalwareFamily> _malwareFamilyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateThreatEventCommandHandler(
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

    public async Task Handle(UpdateThreatEventCommand request, CancellationToken ct)
    {
        ThreatEvent threatEvent = await _threatEventRepository.GetByIdAsync(request.Id, ct) ??
            throw new NotFoundException($"ThreatEvent with ID {request.Id} not found");

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

        request.Adapt(threatEvent);
        threatEvent.UpdatedAt = DateTime.UtcNow;

        await _threatEventRepository.UpdateAsync(threatEvent, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}
