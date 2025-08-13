using Mapster;
using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetThreatEvent;

public class GetThreatEventQueryHandler : IRequestHandler<GetThreatEventQuery, ThreatEventDto>
{
    private readonly IRepository<ThreatEvent> _threatEventRepository;

    public GetThreatEventQueryHandler(IRepository<ThreatEvent> threatEventRepository)
    {
        _threatEventRepository = threatEventRepository;
    }

    public async Task<ThreatEventDto> Handle(GetThreatEventQuery request, CancellationToken ct)
    {
        ThreatEvent threatEvent = await _threatEventRepository.GetByIdAsync(request.Id, ct) ??
            throw new NotFoundException($"ThreatEvent with ID {request.Id} not found");

        return threatEvent.Adapt<ThreatEventDto>();
    }
}
