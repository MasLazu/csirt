using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatIntelligentOverview;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.TenantThreatIntelligentOverview.Queries.GetTopSourceCountries;

public class GetTenantTopSourceCountriesQueryHandler : IRequestHandler<GetTenantTopSourceCountriesQuery, List<TopCountryDto>>
{
    private readonly ITenantThreatIntelligentOverviewRepository _repository;

    public GetTenantTopSourceCountriesQueryHandler(ITenantThreatIntelligentOverviewRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<TopCountryDto>> Handle(GetTenantTopSourceCountriesQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetTopSourceCountriesAsync(request.TenantId, request.StartTime, request.EndTime, request.Limit, cancellationToken);
    }
}