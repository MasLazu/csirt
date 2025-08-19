using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatTemporal;

namespace MeUi.Application.Features.ThreatTemporal.Queries.GetCampaignDuration;

public class GetCampaignDurationHandler : IRequestHandler<GetCampaignDurationQuery, List<CampaignDurationDto>>
{
    private readonly IThreatTemporalRepository _repo;

    public GetCampaignDurationHandler(IThreatTemporalRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<CampaignDurationDto>> Handle(GetCampaignDurationQuery request, CancellationToken cancellationToken)
    {
        return await _repo.GetAttackCampaignDurationAsync(request.Start, request.End, cancellationToken);
    }
}
