using FluentValidation;

namespace MeUi.Application.Features.TenantThreatIncident.Queries.GetIncidentSeverityDistribution;

public class GetTenantIncidentSeverityDistributionQueryValidator : AbstractValidator<GetTenantIncidentSeverityDistributionQuery>
{
    public GetTenantIncidentSeverityDistributionQueryValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty()
            .WithMessage("TenantId is required.");

        RuleFor(x => x.Start)
            .LessThan(x => x.End)
            .WithMessage("Start must be before End.");
    }
}