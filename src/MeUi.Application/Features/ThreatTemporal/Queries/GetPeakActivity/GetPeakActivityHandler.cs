using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatTemporal;

namespace MeUi.Application.Features.ThreatTemporal.Queries.GetPeakActivity;

public class GetPeakActivityHandler : IRequestHandler<GetPeakActivityQuery, List<PeakActivityDto>>
{
    private readonly IThreatTemporalRepository _repo;

    public GetPeakActivityHandler(IThreatTemporalRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<PeakActivityDto>> Handle(GetPeakActivityQuery request, CancellationToken cancellationToken)
    {
        return await _repo.GetPeakActivityByCategoryAsync(request.Start, request.End, cancellationToken);
    }
}
