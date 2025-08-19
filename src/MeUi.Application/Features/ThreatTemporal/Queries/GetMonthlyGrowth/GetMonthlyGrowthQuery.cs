using MediatR;
using MeUi.Application.Models.ThreatTemporal;

namespace MeUi.Application.Features.ThreatTemporal.Queries.GetMonthlyGrowth;

public class GetMonthlyGrowthQuery : IRequest<List<MonthlyGrowthDto>>
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
}
