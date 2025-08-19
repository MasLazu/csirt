using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatNetwork;

namespace MeUi.Application.Features.ThreatNetwork.Queries.GetMostTargetedPorts;

public class GetMostTargetedPortsHandler : IRequestHandler<GetMostTargetedPortsQuery, List<TargetedPortDto>>
{
    private readonly IThreatNetworkRepository _repo;

    public GetMostTargetedPortsHandler(IThreatNetworkRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<TargetedPortDto>> Handle(GetMostTargetedPortsQuery request, CancellationToken cancellationToken)
    {
        return await _repo.GetMostTargetedPortsAsync(request.Start, request.End, request.Limit, cancellationToken);
    }
}
