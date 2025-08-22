using FluentValidation;

namespace MeUi.Application.Features.TenantThreatIntelligentOverview.Queries.GetProtocolDistribution;

public class GetTenantProtocolDistributionQueryValidator : AbstractValidator<GetTenantProtocolDistributionQuery>
{
    public GetTenantProtocolDistributionQueryValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty()
            .WithMessage("TenantId is required.");

        RuleFor(x => x.StartTime)
            .LessThan(x => x.EndTime)
            .WithMessage("StartTime must be before EndTime.");
    }
}