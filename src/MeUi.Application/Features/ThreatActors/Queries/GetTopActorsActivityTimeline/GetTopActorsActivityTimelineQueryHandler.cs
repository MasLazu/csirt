using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatActors;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.ThreatActors.Queries.GetTopActorsActivityTimeline;

public class GetTopActorsActivityTimelineQueryHandler : IRequestHandler<GetTopActorsActivityTimelineQuery, List<ActorActivityTimelineDto>>
{
    private readonly IThreatActorsRepository _repository;

    public GetTopActorsActivityTimelineQueryHandler(IThreatActorsRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ActorActivityTimelineDto>> Handle(GetTopActorsActivityTimelineQuery request, CancellationToken cancellationToken)
    {
        var res = await _repository.GetTopActorsActivityTimelineAsync(request.Start, request.End, request.Interval, request.Limit, cancellationToken);
        return res;
    }
}
