using MediatR;
using MeUi.Application.Models.ThreatNetwork;

namespace MeUi.Application.Features.ThreatNetwork.Queries.GetMostTargetedPorts;

public class GetMostTargetedPortsQuery : IRequest<List<TargetedPortDto>>
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public int Limit { get; set; } = 15;
}
