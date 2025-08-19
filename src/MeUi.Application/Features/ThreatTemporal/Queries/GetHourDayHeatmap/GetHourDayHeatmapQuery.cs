using MediatR;
using MeUi.Application.Models.ThreatTemporal;

namespace MeUi.Application.Features.ThreatTemporal.Queries.GetHourDayHeatmap;

public class GetHourDayHeatmapQuery : IRequest<List<HourDayHeatmapDto>>
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
}
