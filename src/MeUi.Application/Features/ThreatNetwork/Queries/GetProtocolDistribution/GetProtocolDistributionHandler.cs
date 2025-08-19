using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatNetwork;

namespace MeUi.Application.Features.ThreatNetwork.Queries.GetProtocolDistribution;

public class GetProtocolDistributionHandler : IRequestHandler<GetProtocolDistributionQuery, List<ProtocolDistributionDto>>
{
    private readonly IThreatNetworkRepository _repo;

    public GetProtocolDistributionHandler(IThreatNetworkRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<ProtocolDistributionDto>> Handle(GetProtocolDistributionQuery request, CancellationToken cancellationToken)
    {
        return await _repo.GetProtocolDistributionAsync(request.Start, request.End, cancellationToken);
    }
}
