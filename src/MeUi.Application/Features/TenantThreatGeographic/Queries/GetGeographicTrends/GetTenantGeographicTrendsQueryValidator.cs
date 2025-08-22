using FluentValidation;

namespace MeUi.Application.Features.TenantThreatGeographic.Queries.GetGeographicTrends;

public class GetTenantGeographicTrendsQueryValidator : AbstractValidator<GetTenantGeographicTrendsQuery>
{
    public GetTenantGeographicTrendsQueryValidator()
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
    }
}