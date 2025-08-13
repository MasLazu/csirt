using FluentValidation;

namespace MeUi.Application.Features.Protocols.Commands.UpdateProtocol;

public class UpdateProtocolCommandValidator : AbstractValidator<UpdateProtocolCommand>
{
    public UpdateProtocolCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}
