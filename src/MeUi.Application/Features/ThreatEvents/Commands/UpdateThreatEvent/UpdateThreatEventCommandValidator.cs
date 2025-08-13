using FluentValidation;
using System.Net;

namespace MeUi.Application.Features.ThreatEvents.Commands.UpdateThreatEvent;

public class UpdateThreatEventCommandValidator : AbstractValidator<UpdateThreatEventCommand>
{
    public UpdateThreatEventCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("ID is required");

        RuleFor(x => x.AsnRegistryId)
            .NotEmpty()
            .WithMessage("ASN Registry ID is required");

        RuleFor(x => x.SourceAddress)
            .NotEqual(IPAddress.None)
            .WithMessage("Valid source IP address is required");

        RuleFor(x => x.Category)
            .NotEmpty()
            .Length(2, 50)
            .WithMessage("Category must be between 2 and 50 characters");

        RuleFor(x => x.Timestamp)
            .NotEmpty()
            .LessThanOrEqualTo(DateTime.UtcNow.AddHours(1))
            .WithMessage("Timestamp cannot be more than 1 hour in the future");

        RuleFor(x => x.SourcePort)
            .InclusiveBetween(1, 65535)
            .When(x => x.SourcePort.HasValue)
            .WithMessage("Source port must be between 1 and 65535");

        RuleFor(x => x.DestinationPort)
            .InclusiveBetween(1, 65535)
            .When(x => x.DestinationPort.HasValue)
            .WithMessage("Destination port must be between 1 and 65535");
    }
}
