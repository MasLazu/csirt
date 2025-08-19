using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatTemporal;

namespace MeUi.Application.Features.ThreatTemporal.Queries.GetWeekdayWeekend;

public class GetWeekdayWeekendHandler : IRequestHandler<GetWeekdayWeekendQuery, List<TimeSeriesPointDto>>
{
    private readonly IThreatTemporalRepository _repo;

    public GetWeekdayWeekendHandler(IThreatTemporalRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<TimeSeriesPointDto>> Handle(GetWeekdayWeekendQuery request, CancellationToken cancellationToken)
    {
        return await _repo.GetWeekdayWeekendTrendsAsync(request.Start, request.End, cancellationToken);
    }
}
