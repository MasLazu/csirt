using FluentValidation;

namespace MeUi.Application.Features.ThreatIncident.Queries.GetSeverityDistribution;

public class GetSeverityDistributionQueryValidator : AbstractValidator<GetSeverityDistributionQuery>
{
    public GetSeverityDistributionQueryValidator()
    {
        RuleFor(x => x.Start).LessThanOrEqualTo(x => x.End).WithMessage("Start must be before or equal to End");
    }
}
