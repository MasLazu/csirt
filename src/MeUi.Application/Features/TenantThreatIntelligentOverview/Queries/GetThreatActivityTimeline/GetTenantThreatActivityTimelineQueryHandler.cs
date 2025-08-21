using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatIntelligentOverview;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.TenantThreatIntelligentOverview.Queries.GetThreatActivityTimeline;

public class GetTenantThreatActivityTimelineQueryHandler : IRequestHandler<GetTenantThreatActivityTimelineQuery, List<TimelineDataPointDto>>
{
    private readonly ITenantThreatIntelligentOverviewRepository _repository;

    public GetTenantThreatActivityTimelineQueryHandler(ITenantThreatIntelligentOverviewRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<TimelineDataPointDto>> Handle(GetTenantThreatActivityTimelineQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetThreatActivityTimelineAsync(request.TenantId, request.StartTime, request.EndTime, request.Interval, cancellationToken);
    }
}