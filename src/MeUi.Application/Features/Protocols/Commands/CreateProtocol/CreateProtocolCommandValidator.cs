using FluentValidation;

namespace MeUi.Application.Features.Protocols.Commands.CreateProtocol;

public class CreateProtocolCommandValidator : AbstractValidator<CreateProtocolCommand>
{
    public CreateProtocolCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);
    }
}
