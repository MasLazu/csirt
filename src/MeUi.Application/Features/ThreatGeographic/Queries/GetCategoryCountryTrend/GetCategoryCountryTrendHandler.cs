using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatGeographic;

namespace MeUi.Application.Features.ThreatGeographic.Queries.GetCategoryCountryTrend;

public class GetCategoryCountryTrendHandler : IRequestHandler<GetCategoryCountryTrendQuery, List<CategoryCountryTrendPointDto>>
{
    private readonly IThreatGeographicRepository _repo;

    public GetCategoryCountryTrendHandler(IThreatGeographicRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<CategoryCountryTrendPointDto>> Handle(GetCategoryCountryTrendQuery request, CancellationToken cancellationToken)
    {
        return await _repo.GetCategoryCountryTrendAsync(request.Start, request.End, cancellationToken);
    }
}
