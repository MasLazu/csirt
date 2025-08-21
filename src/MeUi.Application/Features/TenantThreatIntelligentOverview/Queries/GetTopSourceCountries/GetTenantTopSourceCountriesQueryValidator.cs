using FluentValidation;

namespace MeUi.Application.Features.TenantThreatIntelligentOverview.Queries.GetTopSourceCountries;

public class GetTenantTopSourceCountriesQueryValidator : AbstractValidator<GetTenantTopSourceCountriesQuery>
{
    public GetTenantTopSourceCountriesQueryValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty()
            .WithMessage("TenantId is required.");

        RuleFor(x => x.StartTime)
            .LessThan(x => x.EndTime)
            .WithMessage("StartTime must be before EndTime.");

        RuleFor(x => x.Limit)
            .GreaterThan(0)
            .LessThanOrEqualTo(100)
            .WithMessage("Limit must be between 1 and 100.");
    }
}