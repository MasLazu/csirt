using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatTemporal;

namespace MeUi.Application.Features.ThreatTemporal.Queries.GetWeeklyAttackDistribution;

public class GetWeeklyAttackDistributionHandler : IRequestHandler<GetWeeklyAttackDistributionQuery, List<DayOfWeekDto>>
{
    private readonly IThreatTemporalRepository _repo;

    public GetWeeklyAttackDistributionHandler(IThreatTemporalRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<DayOfWeekDto>> Handle(GetWeeklyAttackDistributionQuery request, CancellationToken cancellationToken)
    {
        return await _repo.GetWeeklyAttackDistributionAsync(request.Start, request.End, cancellationToken);
    }
}
