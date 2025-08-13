using System.Net;
using MediatR;

namespace MeUi.Application.Features.ThreatEvents.Commands.UpdateThreatEvent;

public record UpdateThreatEventCommand : IRequest
{
    public Guid Id { get; init; }
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
