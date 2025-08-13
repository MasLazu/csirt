using FluentValidation;

namespace MeUi.Application.Features.ThreatEvents.Commands.DeleteThreatEvent;

public class DeleteThreatEventCommandValidator : AbstractValidator<DeleteThreatEventCommand>
{
    public DeleteThreatEventCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("ID is required");
    }
}
