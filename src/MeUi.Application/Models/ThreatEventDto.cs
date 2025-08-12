using System.Net;

namespace MeUi.Application.Models;

public class ThreatEvent : BaseDto
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

    public AsnRegistryDto AsnRegistry { get; set; } = null!;
    public CountryDto? SourceCountry { get; set; }
    public CountryDto? DestinationCountry { get; set; }
    public ProtocolDto? Protocol { get; set; }
    public MalwareFamilyDto? MalwareFamily { get; set; }
}