using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatNetwork;

namespace MeUi.Application.Features.ThreatNetwork.Queries.GetProtocolTrends;

public class GetProtocolTrendsHandler : IRequestHandler<GetProtocolTrendsQuery, List<ProtocolTrendDto>>
{
    private readonly IThreatNetworkRepository _repo;

    public GetProtocolTrendsHandler(IThreatNetworkRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<ProtocolTrendDto>> Handle(GetProtocolTrendsQuery request, CancellationToken cancellationToken)
    {
        return await _repo.GetProtocolTrendsAsync(request.Start, request.End, cancellationToken);
    }
}
