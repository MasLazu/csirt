using FluentValidation;

namespace MeUi.Application.Features.Protocols.Queries.GetProtocol;

public class GetProtocolQueryValidator : AbstractValidator<GetProtocolQuery>
{
    public GetProtocolQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
