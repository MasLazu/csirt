using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatNetwork;

namespace MeUi.Application.Features.ThreatNetwork.Queries.GetTargetedInfrastructure;

public class GetTargetedInfrastructureHandler : IRequestHandler<GetTargetedInfrastructureQuery, List<TargetedInfrastructureDto>>
{
    private readonly IThreatNetworkRepository _repo;

    public GetTargetedInfrastructureHandler(IThreatNetworkRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<TargetedInfrastructureDto>> Handle(GetTargetedInfrastructureQuery request, CancellationToken cancellationToken)
    {
        return await _repo.GetMostTargetedInfrastructureAsync(request.Start, request.End, request.Limit, cancellationToken);
    }
}
