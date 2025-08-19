using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatGeographic;

namespace MeUi.Application.Features.ThreatGeographic.Queries.GetCountryRankings;

public class GetCountryRankingsHandler : IRequestHandler<GetCountryRankingsQuery, List<CountryAttackRankingDto>>
{
    private readonly IThreatGeographicRepository _repo;

    public GetCountryRankingsHandler(IThreatGeographicRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<CountryAttackRankingDto>> Handle(GetCountryRankingsQuery request, CancellationToken cancellationToken)
    {
        return await _repo.GetCountryRankingsAsync(request.Start, request.End, request.Limit, cancellationToken);
    }
}
