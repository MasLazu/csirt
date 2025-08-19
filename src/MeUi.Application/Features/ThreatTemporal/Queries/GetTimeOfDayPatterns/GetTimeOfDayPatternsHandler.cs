using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatTemporal;

namespace MeUi.Application.Features.ThreatTemporal.Queries.GetTimeOfDayPatterns;

public class GetTimeOfDayPatternsHandler : IRequestHandler<GetTimeOfDayPatternsQuery, List<TimePeriodSeriesDto>>
{
    private readonly IThreatTemporalRepository _repo;

    public GetTimeOfDayPatternsHandler(IThreatTemporalRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<TimePeriodSeriesDto>> Handle(GetTimeOfDayPatternsQuery request, CancellationToken cancellationToken)
    {
        return await _repo.GetAttackPatternsByTimeOfDayAsync(request.Start, request.End, cancellationToken);
    }
}
