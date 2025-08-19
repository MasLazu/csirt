using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatGeographic;

namespace MeUi.Application.Features.ThreatGeographic.Queries.GetCountryAttackTrends;

public class GetCountryAttackTrendsHandler : IRequestHandler<GetCountryAttackTrendsQuery, List<CountryAttackTrendPointDto>>
{
    private readonly IThreatGeographicRepository _repo;

    public GetCountryAttackTrendsHandler(IThreatGeographicRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<CountryAttackTrendPointDto>> Handle(GetCountryAttackTrendsQuery request, CancellationToken cancellationToken)
    {
        return await _repo.GetCountryAttackTrendsAsync(request.Start, request.End, cancellationToken);
    }
}
