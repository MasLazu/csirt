using FluentValidation;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventGeoHeatmapAnalytics;

public class GetThreatEventGeoHeatmapAnalyticsQueryValidator : AbstractValidator<GetThreatEventGeoHeatmapAnalyticsQuery>
{
    public GetThreatEventGeoHeatmapAnalyticsQueryValidator()
    {
        RuleFor(x => x.EndTime)
            .GreaterThan(x => x.StartTime)
            .When(x => x.StartTime.HasValue && x.EndTime.HasValue)
            .WithMessage("End time must be after start time");

        RuleFor(x => x.TopCountriesLimit)
            .GreaterThan(0)
            .LessThanOrEqualTo(250)
            .WithMessage("Top countries limit must be between 1 and 250");
    }
}
