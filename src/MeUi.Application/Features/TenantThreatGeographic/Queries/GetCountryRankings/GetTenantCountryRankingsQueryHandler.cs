using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatGeographic;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.TenantThreatGeographic.Queries.GetCountryRankings;

public class GetTenantCountryRankingsQueryHandler : IRequestHandler<GetTenantCountryRankingsQuery, List<CountryRankingDto>>
{
    private readonly ITenantThreatGeographicRepository _repository;

    public GetTenantCountryRankingsQueryHandler(ITenantThreatGeographicRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<CountryRankingDto>> Handle(GetTenantCountryRankingsQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetCountryRankingsAsync(request.TenantId, request.Start, request.End, request.Limit, cancellationToken);
    }
}