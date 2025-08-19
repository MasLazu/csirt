using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatNetwork;

namespace MeUi.Application.Features.ThreatNetwork.Queries.GetHighRiskIps;

public class GetHighRiskIpsHandler : IRequestHandler<GetHighRiskIpsQuery, List<HighRiskIpDto>>
{
    private readonly IThreatNetworkRepository _repo;

    public GetHighRiskIpsHandler(IThreatNetworkRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<HighRiskIpDto>> Handle(GetHighRiskIpsQuery request, CancellationToken cancellationToken)
    {
        return await _repo.GetHighRiskIpReputationAsync(request.Start, request.End, request.Limit, cancellationToken);
    }
}
