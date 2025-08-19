using FluentValidation;

namespace MeUi.Application.Features.ThreatNetwork.Queries.GetProtocolTrends;

public class GetProtocolTrendsQueryValidator : AbstractValidator<GetProtocolTrendsQuery>
{
    public GetProtocolTrendsQueryValidator()
    {
        RuleFor(x => x.Start).LessThan(x => x.End);
    }
}
