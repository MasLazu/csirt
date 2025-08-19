using MediatR;
using MeUi.Application.Models.ThreatNetwork;

namespace MeUi.Application.Features.ThreatNetwork.Queries.GetAsnNetwork;

public class GetAsnNetworkQuery : IRequest<List<AsnNetworkDto>>
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public int Limit { get; set; } = 20;
}
