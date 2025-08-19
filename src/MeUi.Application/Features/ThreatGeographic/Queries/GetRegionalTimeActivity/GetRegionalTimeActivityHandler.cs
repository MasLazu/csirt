using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatGeographic;

namespace MeUi.Application.Features.ThreatGeographic.Queries.GetRegionalTimeActivity;

public class GetRegionalTimeActivityHandler : IRequestHandler<GetRegionalTimeActivityQuery, List<RegionalTimeBucketDto>>
{
    private readonly IThreatGeographicRepository _repo;

    public GetRegionalTimeActivityHandler(IThreatGeographicRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<RegionalTimeBucketDto>> Handle(GetRegionalTimeActivityQuery request, CancellationToken cancellationToken)
    {
        return await _repo.GetRegionalTimeActivityAsync(request.Start, request.End, request.Limit, cancellationToken);
    }
}
