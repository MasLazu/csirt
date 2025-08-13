using FluentValidation;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventTimelineAnalytics;

public class GetThreatEventTimelineAnalyticsQueryValidator : AbstractValidator<GetThreatEventTimelineAnalyticsQuery>
{
    public GetThreatEventTimelineAnalyticsQueryValidator()
    {
        RuleFor(x => x.StartTime)
            .NotEmpty()
            .WithMessage("Start time is required for analytics queries");

        RuleFor(x => x.EndTime)
            .NotEmpty()
            .WithMessage("End time is required for analytics queries")
            .GreaterThan(x => x.StartTime)
            .WithMessage("End time must be after start time");

        RuleFor(x => x.Interval)
            .NotEmpty()
            .Must(BeValidInterval)
            .WithMessage("Interval must be one of: hour, day, week, month");

        RuleFor(x => x.TopItemsLimit)
            .GreaterThan(0)
            .LessThanOrEqualTo(100)
            .WithMessage("Top items limit must be between 1 and 100");

        // Time range validation for performance
        RuleFor(x => x)
            .Must(HaveReasonableTimeRange)
            .WithMessage("Time range is too large for the selected interval. Maximum ranges: hour (7 days), day (1 year), week (5 years), month (10 years)");

        // Category validation
        RuleFor(x => x.Category)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.Category))
            .WithMessage("Category must not exceed 100 characters");
    }

    private static bool BeValidInterval(string interval)
    {
        return interval?.ToLower() is "hour" or "day" or "week" or "month";
    }

    private static bool HaveReasonableTimeRange(GetThreatEventTimelineAnalyticsQuery query)
    {
        TimeSpan timeRange = query.EndTime - query.StartTime;

        return query.Interval?.ToLower() switch
        {
            "hour" => timeRange <= TimeSpan.FromDays(7),      // Max 7 days for hourly
            "day" => timeRange <= TimeSpan.FromDays(365),     // Max 1 year for daily
            "week" => timeRange <= TimeSpan.FromDays(1825),   // Max 5 years for weekly
            "month" => timeRange <= TimeSpan.FromDays(3650),  // Max 10 years for monthly
            _ => true
        };
    }
}
