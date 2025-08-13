using MediatR;
using MeUi.Application.Models;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetThreatEvent;

public record GetThreatEventQuery : IRequest<ThreatEventDto>
{
    public Guid Id { get; init; }
}
