using FluentValidation;

namespace MeUi.Application.Features.TenantThreatCompliance.Queries.GetComplianceScore;

public class GetTenantComplianceScoreQueryValidator : AbstractValidator<GetTenantComplianceScoreQuery>
{
    public GetTenantComplianceScoreQueryValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty()
            .WithMessage("TenantId is required.");

        RuleFor(x => x.Start)
            .LessThan(x => x.End)
            .WithMessage("Start must be before End.");
    }
}