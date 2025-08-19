using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatNetwork;

namespace MeUi.Application.Features.ThreatNetwork.Queries.GetAsnNetwork;

public class GetAsnNetworkHandler : IRequestHandler<GetAsnNetworkQuery, List<AsnNetworkDto>>
{
    private readonly IThreatNetworkRepository _repo;

    public GetAsnNetworkHandler(IThreatNetworkRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<AsnNetworkDto>> Handle(GetAsnNetworkQuery request, CancellationToken cancellationToken)
    {
        return await _repo.GetAsnNetworkAnalysisAsync(request.Start, request.End, request.Limit, cancellationToken);
    }
}
