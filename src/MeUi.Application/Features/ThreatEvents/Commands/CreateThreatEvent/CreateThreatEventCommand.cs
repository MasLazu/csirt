using System.Net;
using MediatR;

namespace MeUi.Application.Features.ThreatEvents.Commands.CreateThreatEvent;

public record CreateThreatEventCommand : IRequest<Guid>
{
    public Guid AsnRegistryId { get; init; }
    public IPAddress SourceAddress { get; init; } = IPAddress.None;
    public Guid? SourceCountryId { get; init; }
    public IPAddress? DestinationAddress { get; init; }
    public Guid? DestinationCountryId { get; init; }
    public int? SourcePort { get; init; }
    public int? DestinationPort { get; init; }
    public Guid? ProtocolId { get; init; }
    public string Category { get; init; } = string.Empty;
    public Guid? MalwareFamilyId { get; init; }
    public DateTime Timestamp { get; init; }
}
