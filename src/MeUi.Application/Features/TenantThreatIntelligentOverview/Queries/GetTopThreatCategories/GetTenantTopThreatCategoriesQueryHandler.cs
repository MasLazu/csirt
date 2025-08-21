using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatIntelligentOverview;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.TenantThreatIntelligentOverview.Queries.GetTopThreatCategories;

public class GetTenantTopThreatCategoriesQueryHandler : IRequestHandler<GetTenantTopThreatCategoriesQuery, List<TopCategoryDto>>
{
    private readonly ITenantThreatIntelligentOverviewRepository _repository;

    public GetTenantTopThreatCategoriesQueryHandler(ITenantThreatIntelligentOverviewRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<TopCategoryDto>> Handle(GetTenantTopThreatCategoriesQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetTopThreatCategoriesAsync(request.TenantId, request.StartTime, request.EndTime, request.Limit, cancellationToken);
    }
}