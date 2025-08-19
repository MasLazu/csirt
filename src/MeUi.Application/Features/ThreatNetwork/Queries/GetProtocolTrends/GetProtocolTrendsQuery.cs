using MediatR;
using MeUi.Application.Models.ThreatNetwork;

namespace MeUi.Application.Features.ThreatNetwork.Queries.GetProtocolTrends;

public class GetProtocolTrendsQuery : IRequest<List<ProtocolTrendDto>>
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
}
