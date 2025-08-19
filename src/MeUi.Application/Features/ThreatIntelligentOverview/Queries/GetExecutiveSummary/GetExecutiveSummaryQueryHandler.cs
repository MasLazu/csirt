using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatIntelligentOverview;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.ThreatIntelligentOverview.Queries.GetExecutiveSummary;

public class GetExecutiveSummaryQueryHandler : IRequestHandler<GetExecutiveSummaryQuery, List<ExecutiveSummaryMetricDto>>
{
    private readonly IThreatIntelligentOverviewRepository _repository;

    public GetExecutiveSummaryQueryHandler(IThreatIntelligentOverviewRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ExecutiveSummaryMetricDto>> Handle(GetExecutiveSummaryQuery request, CancellationToken cancellationToken)
    {
        var metrics = await _repository.GetExecutiveSummaryAsync(request.StartTime, request.EndTime, cancellationToken);
        return metrics;
    }
}
