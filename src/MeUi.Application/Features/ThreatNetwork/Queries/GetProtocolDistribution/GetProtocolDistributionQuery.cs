using MediatR;
using MeUi.Application.Models.ThreatNetwork;

namespace MeUi.Application.Features.ThreatNetwork.Queries.GetProtocolDistribution;

public class GetProtocolDistributionQuery : IRequest<List<ProtocolDistributionDto>>
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
}
