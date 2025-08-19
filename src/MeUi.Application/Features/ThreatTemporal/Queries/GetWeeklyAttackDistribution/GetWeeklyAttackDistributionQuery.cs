using MediatR;
using MeUi.Application.Models.ThreatTemporal;

namespace MeUi.Application.Features.ThreatTemporal.Queries.GetWeeklyAttackDistribution;

public class GetWeeklyAttackDistributionQuery : IRequest<List<DayOfWeekDto>>
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
}
