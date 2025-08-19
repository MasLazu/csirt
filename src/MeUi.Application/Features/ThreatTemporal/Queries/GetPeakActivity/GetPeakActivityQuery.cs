using MediatR;
using MeUi.Application.Models.ThreatTemporal;

namespace MeUi.Application.Features.ThreatTemporal.Queries.GetPeakActivity;

public class GetPeakActivityQuery : IRequest<List<PeakActivityDto>>
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
}
