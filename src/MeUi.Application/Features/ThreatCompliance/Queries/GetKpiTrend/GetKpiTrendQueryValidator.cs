using FluentValidation;

namespace MeUi.Application.Features.ThreatCompliance.Queries.GetKpiTrend;

public class GetKpiTrendQueryValidator : AbstractValidator<GetKpiTrendQuery>
{
    public GetKpiTrendQueryValidator()
    {
        RuleFor(x => x.Start)
            .LessThanOrEqualTo(x => x.End)
            .WithMessage("Start must be before or equal to End");
    }
}
