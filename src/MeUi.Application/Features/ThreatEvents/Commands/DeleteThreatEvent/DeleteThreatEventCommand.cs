using MediatR;

namespace MeUi.Application.Features.ThreatEvents.Commands.DeleteThreatEvent;

public record DeleteThreatEventCommand : IRequest
{
    public Guid Id { get; init; }
}
