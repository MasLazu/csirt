using FluentValidation;

namespace MeUi.Application.Features.Protocols.Commands.DeleteProtocol;

public class DeleteProtocolCommandValidator : AbstractValidator<DeleteProtocolCommand>
{
    public DeleteProtocolCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
