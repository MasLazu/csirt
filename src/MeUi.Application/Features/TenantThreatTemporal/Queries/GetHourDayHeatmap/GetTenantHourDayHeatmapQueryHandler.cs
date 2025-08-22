using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatTemporal;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.TenantThreatTemporal.Queries.GetHourDayHeatmap;

public class GetTenantHourDayHeatmapQueryHandler : IRequestHandler<GetTenantHourDayHeatmapQuery, List<HourDayHeatmapDto>>
{
    private readonly ITenantThreatTemporalRepository _repository;

    public GetTenantHourDayHeatmapQueryHandler(ITenantThreatTemporalRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<HourDayHeatmapDto>> Handle(GetTenantHourDayHeatmapQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetHourDayHeatmapAsync(
            request.TenantId,
            request.Start,
            request.End,
            cancellationToken);
    }
}