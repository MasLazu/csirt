using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatGeographic;

namespace MeUi.Application.Features.ThreatGeographic.Queries.GetCrossBorderFlows;

public class GetCrossBorderFlowsHandler : IRequestHandler<GetCrossBorderFlowsQuery, List<CrossBorderFlowDto>>
{
    private readonly IThreatGeographicRepository _repo;

    public GetCrossBorderFlowsHandler(IThreatGeographicRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<CrossBorderFlowDto>> Handle(GetCrossBorderFlowsQuery request, CancellationToken cancellationToken)
    {
        return await _repo.GetCrossBorderFlowsAsync(request.Start, request.End, request.Limit, cancellationToken);
    }
}
