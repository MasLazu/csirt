using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatTemporal;

namespace MeUi.Application.Features.ThreatTemporal.Queries.GetHourDayHeatmap;

public class GetHourDayHeatmapHandler : IRequestHandler<GetHourDayHeatmapQuery, List<HourDayHeatmapDto>>
{
    private readonly IThreatTemporalRepository _repo;

    public GetHourDayHeatmapHandler(IThreatTemporalRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<HourDayHeatmapDto>> Handle(GetHourDayHeatmapQuery request, CancellationToken cancellationToken)
    {
        return await _repo.GetHourDayHeatmapAsync(request.Start, request.End, cancellationToken);
    }
}
