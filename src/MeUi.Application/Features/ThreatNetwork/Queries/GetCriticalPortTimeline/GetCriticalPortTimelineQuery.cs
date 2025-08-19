using MediatR;
using MeUi.Application.Models.ThreatNetwork;

namespace MeUi.Application.Features.ThreatNetwork.Queries.GetCriticalPortTimeline;

public class GetCriticalPortTimelineQuery : IRequest<List<CriticalPortTimePointDto>>
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
}
