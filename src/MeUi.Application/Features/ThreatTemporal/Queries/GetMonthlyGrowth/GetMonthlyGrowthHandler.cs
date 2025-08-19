using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatTemporal;

namespace MeUi.Application.Features.ThreatTemporal.Queries.GetMonthlyGrowth;

public class GetMonthlyGrowthHandler : IRequestHandler<GetMonthlyGrowthQuery, List<MonthlyGrowthDto>>
{
    private readonly IThreatTemporalRepository _repo;

    public GetMonthlyGrowthHandler(IThreatTemporalRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<MonthlyGrowthDto>> Handle(GetMonthlyGrowthQuery request, CancellationToken cancellationToken)
    {
        return await _repo.GetMonthlyGrowthRateAsync(request.Start, request.End, cancellationToken);
    }
}
