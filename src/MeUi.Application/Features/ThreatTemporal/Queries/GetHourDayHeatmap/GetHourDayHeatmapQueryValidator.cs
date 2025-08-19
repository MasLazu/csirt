using FluentValidation;

namespace MeUi.Application.Features.ThreatTemporal.Queries.GetHourDayHeatmap;

public class GetHourDayHeatmapQueryValidator : AbstractValidator<GetHourDayHeatmapQuery>
{
    public GetHourDayHeatmapQueryValidator()
    {
        RuleFor(x => x.Start).LessThan(x => x.End);
    }
}
