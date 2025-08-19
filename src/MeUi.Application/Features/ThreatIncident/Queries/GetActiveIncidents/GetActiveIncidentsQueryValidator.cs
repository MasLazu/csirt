using FluentValidation;

namespace MeUi.Application.Features.ThreatIncident.Queries.GetActiveIncidents;

public class GetActiveIncidentsQueryValidator : AbstractValidator<GetActiveIncidentsQuery>
{
    public GetActiveIncidentsQueryValidator()
    {
        RuleFor(x => x.Start).LessThanOrEqualTo(x => x.End).WithMessage("Start must be before or equal to End");
        RuleFor(x => x.Limit).GreaterThan(0).WithMessage("Limit must be greater than 0");
    }
}
