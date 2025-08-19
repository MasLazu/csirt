using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatIntelligentOverview;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.ThreatIntelligentOverview.Queries.GetThreatCategoryAnalysis;

public class GetThreatCategoryAnalysisQueryHandler : IRequestHandler<GetThreatCategoryAnalysisQuery, List<ThreatCategoryAnalysisDto>>
{
    private readonly IThreatIntelligentOverviewRepository _repository;

    public GetThreatCategoryAnalysisQueryHandler(IThreatIntelligentOverviewRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ThreatCategoryAnalysisDto>> Handle(GetThreatCategoryAnalysisQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetThreatCategoryAnalysisAsync(request.StartTime, request.EndTime, cancellationToken);
    }
}
