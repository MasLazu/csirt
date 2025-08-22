using FluentValidation;

namespace MeUi.Application.Features.TenantThreatCompliance.Queries.GetKpiTrend;

public class GetTenantKpiTrendQueryValidator : AbstractValidator<GetTenantKpiTrendQuery>
{
    public GetTenantKpiTrendQueryValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty()
            .WithMessage("TenantId is required.");

        RuleFor(x => x.Start)
            .LessThan(x => x.End)
            .WithMessage("Start must be before End.");
    }
}