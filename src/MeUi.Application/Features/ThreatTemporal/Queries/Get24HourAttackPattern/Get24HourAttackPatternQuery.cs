using MediatR;
using MeUi.Application.Models.ThreatTemporal;

namespace MeUi.Application.Features.ThreatTemporal.Queries.Get24HourAttackPattern;

public class Get24HourAttackPatternQuery : IRequest<List<TimeSeriesPointDto>>
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
}
