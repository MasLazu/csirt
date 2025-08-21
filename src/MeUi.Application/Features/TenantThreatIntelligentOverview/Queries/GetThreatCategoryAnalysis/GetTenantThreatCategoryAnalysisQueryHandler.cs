using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatIntelligentOverview;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.TenantThreatIntelligentOverview.Queries.GetThreatCategoryAnalysis;

public class GetTenantThreatCategoryAnalysisQueryHandler : IRequestHandler<GetTenantThreatCategoryAnalysisQuery, List<ThreatCategoryAnalysisDto>>
{
    private readonly ITenantThreatIntelligentOverviewRepository _repository;

    public GetTenantThreatCategoryAnalysisQueryHandler(ITenantThreatIntelligentOverviewRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ThreatCategoryAnalysisDto>> Handle(GetTenantThreatCategoryAnalysisQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetThreatCategoryAnalysisAsync(request.TenantId, request.StartTime, request.EndTime, cancellationToken);
    }
}