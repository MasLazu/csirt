using FluentValidation;

namespace MeUi.Application.Features.TenantThreatGeographic.Queries.GetCountryCategoryTimeline;

public class GetTenantCountryCategoryTimelineQueryValidator : AbstractValidator<GetTenantCountryCategoryTimelineQuery>
{
    public GetTenantCountryCategoryTimelineQueryValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty()
            .WithMessage("TenantId is required.");

        RuleFor(x => x.Start)
            .LessThan(x => x.End)
            .WithMessage("Start must be before End.");

        RuleFor(x => x.Interval)
            .GreaterThan(TimeSpan.Zero)
            .WithMessage("Interval must be greater than zero.");

        RuleFor(x => x.TopCountries)
            .GreaterThan(0)
            .LessThanOrEqualTo(20)
            .WithMessage("TopCountries must be between 1 and 20.");
    }
}