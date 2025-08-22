using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatIntelligentOverview;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.TenantThreatIntelligentOverview.Queries.GetExecutiveSummary;

public class GetTenantExecutiveSummaryQueryHandler : IRequestHandler<GetTenantExecutiveSummaryQuery, List<ExecutiveSummaryMetricDto>>
{
    private readonly ITenantThreatIntelligentOverviewRepository _repository;

    public GetTenantExecutiveSummaryQueryHandler(ITenantThreatIntelligentOverviewRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ExecutiveSummaryMetricDto>> Handle(GetTenantExecutiveSummaryQuery request, CancellationToken cancellationToken)
    {
        var metrics = await _repository.GetExecutiveSummaryAsync(request.TenantId, request.StartTime, request.EndTime, cancellationToken);
        return metrics;
    }
}