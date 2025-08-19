using MediatR;
using MeUi.Application.Models.ThreatNetwork;

namespace MeUi.Application.Features.ThreatNetwork.Queries.GetHighRiskIps;

public class GetHighRiskIpsQuery : IRequest<List<HighRiskIpDto>>
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public int Limit { get; set; } = 20;
}
