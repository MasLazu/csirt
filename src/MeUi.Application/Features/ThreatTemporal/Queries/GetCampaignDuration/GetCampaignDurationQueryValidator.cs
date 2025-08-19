using FluentValidation;

namespace MeUi.Application.Features.ThreatTemporal.Queries.GetCampaignDuration;

public class GetCampaignDurationQueryValidator : AbstractValidator<GetCampaignDurationQuery>
{
    public GetCampaignDurationQueryValidator()
    {
        RuleFor(x => x.Start).LessThan(x => x.End);
    }
}
