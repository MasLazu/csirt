using MediatR;
using MeUi.Application.Models.ThreatTemporal;

namespace MeUi.Application.Features.ThreatTemporal.Queries.GetWeekdayWeekend;

public class GetWeekdayWeekendQuery : IRequest<List<TimeSeriesPointDto>>
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
}
