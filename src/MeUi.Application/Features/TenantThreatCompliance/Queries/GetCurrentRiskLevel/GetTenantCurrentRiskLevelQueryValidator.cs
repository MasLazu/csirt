using FluentValidation;

namespace MeUi.Application.Features.TenantThreatCompliance.Queries.GetCurrentRiskLevel;

public class GetTenantCurrentRiskLevelQueryValidator : AbstractValidator<GetTenantCurrentRiskLevelQuery>
{
    public GetTenantCurrentRiskLevelQueryValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty()
            .WithMessage("TenantId is required.");

        RuleFor(x => x.Start)
            .LessThan(x => x.End)
            .WithMessage("Start must be before End.");
    }
}