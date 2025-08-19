using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatIntelligentOverview;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.ThreatIntelligentOverview.Queries.GetTopSourceCountries;

public class GetTopSourceCountriesQueryHandler : IRequestHandler<GetTopSourceCountriesQuery, List<TopCountryDto>>
{
    private readonly IThreatIntelligentOverviewRepository _repository;

    public GetTopSourceCountriesQueryHandler(IThreatIntelligentOverviewRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<TopCountryDto>> Handle(GetTopSourceCountriesQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetTopSourceCountriesAsync(request.StartTime, request.EndTime, request.Limit, cancellationToken);
    }
}
