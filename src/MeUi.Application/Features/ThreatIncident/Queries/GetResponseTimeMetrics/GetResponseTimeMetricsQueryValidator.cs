using FluentValidation;

namespace MeUi.Application.Features.ThreatIncident.Queries.GetResponseTimeMetrics;

public class GetResponseTimeMetricsQueryValidator : AbstractValidator<GetResponseTimeMetricsQuery>
{
    public GetResponseTimeMetricsQueryValidator()
    {
        RuleFor(x => x.Start).LessThanOrEqualTo(x => x.End).WithMessage("Start must be before or equal to End");
    }
}
