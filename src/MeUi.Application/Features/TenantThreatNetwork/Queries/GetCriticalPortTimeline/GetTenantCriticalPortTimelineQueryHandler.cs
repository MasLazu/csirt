using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatNetwork;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.TenantThreatNetwork.Queries.GetCriticalPortTimeline;

public class GetTenantCriticalPortTimelineQueryHandler : IRequestHandler<GetTenantCriticalPortTimelineQuery, List<CriticalPortTimePointDto>>
{
    private readonly ITenantThreatNetworkRepository _repository;

    public GetTenantCriticalPortTimelineQueryHandler(ITenantThreatNetworkRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<CriticalPortTimePointDto>> Handle(GetTenantCriticalPortTimelineQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetCriticalPortTimelineAsync(
            request.TenantId,
            request.Start,
            request.End,
            request.Interval,
            cancellationToken);
    }
}