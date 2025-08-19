using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatTemporal;

namespace MeUi.Application.Features.ThreatTemporal.Queries.Get24HourAttackPattern;

public class Get24HourAttackPatternHandler : IRequestHandler<Get24HourAttackPatternQuery, List<TimeSeriesPointDto>>
{
    private readonly IThreatTemporalRepository _repo;

    public Get24HourAttackPatternHandler(IThreatTemporalRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<TimeSeriesPointDto>> Handle(Get24HourAttackPatternQuery request, CancellationToken cancellationToken)
    {
        return await _repo.Get24HourAttackPatternAsync(request.Start, request.End, cancellationToken);
    }
}
