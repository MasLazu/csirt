using FluentValidation;

namespace MeUi.Application.Features.ThreatTemporal.Queries.GetPeakActivity;

public class GetPeakActivityQueryValidator : AbstractValidator<GetPeakActivityQuery>
{
    public GetPeakActivityQueryValidator()
    {
        RuleFor(x => x.Start).LessThan(x => x.End);
    }
}
