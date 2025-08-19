using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatNetwork;

namespace MeUi.Application.Features.ThreatNetwork.Queries.GetCriticalPortTimeline;

public class GetCriticalPortTimelineHandler : IRequestHandler<GetCriticalPortTimelineQuery, List<CriticalPortTimePointDto>>
{
    private readonly IThreatNetworkRepository _repo;

    public GetCriticalPortTimelineHandler(IThreatNetworkRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<CriticalPortTimePointDto>> Handle(GetCriticalPortTimelineQuery request, CancellationToken cancellationToken)
    {
        return await _repo.GetCriticalPortTimelineAsync(request.Start, request.End, cancellationToken);
    }
}
