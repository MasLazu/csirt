using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatActors;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.TenantThreatActors.Queries.GetTopActorsActivityTimeline;

public class GetTenantTopActorsActivityTimelineQueryHandler : IRequestHandler<GetTenantTopActorsActivityTimelineQuery, List<ActorActivityTimelineDto>>
{
    private readonly ITenantThreatActorsRepository _repository;

    public GetTenantTopActorsActivityTimelineQueryHandler(ITenantThreatActorsRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ActorActivityTimelineDto>> Handle(GetTenantTopActorsActivityTimelineQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetTopActorsActivityTimelineAsync(request.TenantId, request.Start, request.End, request.Interval, request.Limit, cancellationToken);
    }
}