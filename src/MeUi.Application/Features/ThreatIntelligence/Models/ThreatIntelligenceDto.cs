namespace MeUi.Application.Features.ThreatIntelligence.Models;

public class ThreatIntelligenceDto
{
    public Guid Id { get; set; }
    public string Asn { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string AsnInfo { get; set; } = string.Empty;
    public OptionalInformationDto OptionalInformation { get; set; } = new();
    public string Category { get; set; } = string.Empty;
    public string SourceAddress { get; set; } = string.Empty;
    public string SourceCountry { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class OptionalInformationDto
{
    public string? DestinationAddress { get; set; }
    public string? DestinationCountry { get; set; }
    public string? DestinationPort { get; set; }
    public string? SourcePort { get; set; }
    public string? Protocol { get; set; }
    public string? Family { get; set; }
}