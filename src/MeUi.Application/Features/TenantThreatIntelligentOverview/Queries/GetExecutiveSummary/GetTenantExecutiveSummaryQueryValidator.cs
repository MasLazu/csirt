using FluentValidation;

namespace MeUi.Application.Features.TenantThreatIntelligentOverview.Queries.GetExecutiveSummary;

public class GetTenantExecutiveSummaryQueryValidator : AbstractValidator<GetTenantExecutiveSummaryQuery>
{
    public GetTenantExecutiveSummaryQueryValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty()
            .WithMessage("TenantId is required.");

        RuleFor(x => x.StartTime)
            .LessThan(x => x.EndTime)
            .WithMessage("StartTime must be before EndTime.");
    }
}