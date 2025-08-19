using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatIntelligentOverview;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.ThreatIntelligentOverview.Queries.GetThreatActivityTimeline;

public class GetThreatActivityTimelineQueryHandler : IRequestHandler<GetThreatActivityTimelineQuery, List<TimelineDataPointDto>>
{
    private readonly IThreatIntelligentOverviewRepository _repository;

    public GetThreatActivityTimelineQueryHandler(IThreatIntelligentOverviewRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<TimelineDataPointDto>> Handle(GetThreatActivityTimelineQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetThreatActivityTimelineAsync(request.StartTime, request.EndTime, request.Interval, cancellationToken);
    }
}
