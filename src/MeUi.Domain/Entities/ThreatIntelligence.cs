namespace MeUi.Domain.Entities;

public class ThreatIntelligence : BaseEntity
{
    public string Asn { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string AsnInfo { get; set; } = string.Empty;
    public OptionalInformation OptionalInformation { get; set; } = new();
    public string Category { get; set; } = string.Empty;
    public string SourceAddress { get; set; } = string.Empty;
    public string SourceCountry { get; set; } = string.Empty;
}