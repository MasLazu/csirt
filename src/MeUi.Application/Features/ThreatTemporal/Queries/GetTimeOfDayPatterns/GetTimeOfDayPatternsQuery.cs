using MediatR;
using MeUi.Application.Models.ThreatTemporal;

namespace MeUi.Application.Features.ThreatTemporal.Queries.GetTimeOfDayPatterns;

public class GetTimeOfDayPatternsQuery : IRequest<List<TimePeriodSeriesDto>>
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
}
