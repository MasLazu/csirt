using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatIntelligentOverview;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.ThreatIntelligentOverview.Queries.GetTopThreatCategories;

public class GetTopThreatCategoriesQueryHandler : IRequestHandler<GetTopThreatCategoriesQuery, List<TopCategoryDto>>
{
    private readonly IThreatIntelligentOverviewRepository _repository;

    public GetTopThreatCategoriesQueryHandler(IThreatIntelligentOverviewRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<TopCategoryDto>> Handle(GetTopThreatCategoriesQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetTopThreatCategoriesAsync(request.StartTime, request.EndTime, request.Limit, cancellationToken);
    }
}
