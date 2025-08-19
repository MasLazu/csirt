using MediatR;
using MeUi.Application.Models.ThreatTemporal;

namespace MeUi.Application.Features.ThreatTemporal.Queries.GetCampaignDuration;

public class GetCampaignDurationQuery : IRequest<List<CampaignDurationDto>>
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
}
