using FluentValidation;

namespace MeUi.Application.Features.TenantThreatCompliance.Queries.GetExecutiveSummary;

public class GetTenantExecutiveSummaryQueryValidator : AbstractValidator<GetTenantExecutiveSummaryQuery>
{
    public GetTenantExecutiveSummaryQueryValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty()
            .WithMessage("TenantId is required.");

        RuleFor(x => x.Start)
            .LessThan(x => x.End)
            .WithMessage("Start must be before End.");
    }
}