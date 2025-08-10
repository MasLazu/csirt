using System.Net;

namespace MeUi.Domain.Entities;

public class ThreatEvent : BaseEntity
{
    public Guid AsnRegistryId { get; set; }
    public IPAddress SourceAddress { get; set; } = IPAddress.None;
    public Guid? SourceCountryId { get; set; }
    public IPAddress? DestinationAddress { get; set; }
    public Guid? DestinationCountryId { get; set; }
    public int? SourcePort { get; set; }
    public int? DestinationPort { get; set; }
    public Guid? ProtocolId { get; set; }
    public string Category { get; set; } = string.Empty;
    public Guid? MalwareFamilyId { get; set; }
    public DateTime Timestamp { get; set; }

    public AsnRegistry AsnRegistry { get; set; } = null!;
    public Country? SourceCountry { get; set; }
    public Country? DestinationCountry { get; set; }
    public Protocol? Protocol { get; set; }
    public MalwareFamily? MalwareFamily { get; set; }
}