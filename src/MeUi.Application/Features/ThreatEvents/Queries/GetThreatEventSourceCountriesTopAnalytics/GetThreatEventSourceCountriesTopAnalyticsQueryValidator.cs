using FluentValidation;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventSourceCountriesTopAnalytics;

public class GetThreatEventSourceCountriesTopAnalyticsQueryValidator : AbstractValidator<GetThreatEventSourceCountriesTopAnalyticsQuery>
{
    public GetThreatEventSourceCountriesTopAnalyticsQueryValidator()
    {
        RuleFor(x => x.TopLimit).InclusiveBetween(1, 100);
        RuleFor(x => x).Must(HaveValidRange)
            .WithMessage("Invalid time range: StartTime must be before EndTime and window <= 180 days");
    }

    private static bool HaveValidRange(GetThreatEventSourceCountriesTopAnalyticsQuery q)
    {
        if (q.StartTime.HasValue && q.EndTime.HasValue)
        {
            if (q.StartTime > q.EndTime) return false;
            return (q.EndTime.Value - q.StartTime.Value).TotalDays <= 180;
        }
        return true;
    }
}
