namespace MeUi.Application.Features.ThreatIntelligence.Models;

public class ThreatIntelligenceDto
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; }

    // ASN Information
    public string Asn { get; set; } = string.Empty;
    public string AsnInfo { get; set; } = string.Empty;

    // Address Information
    public string SourceAddress { get; set; } = string.Empty;
    public string? DestinationAddress { get; set; }

    // Country Information
    public string? SourceCountry { get; set; }
    public string? DestinationCountry { get; set; }

    // Port Information
    public int? SourcePort { get; set; }
    public int? DestinationPort { get; set; }

    // Protocol and Category
    public string? Protocol { get; set; }
    public string Category { get; set; } = string.Empty;

    // Malware Family
    public string? MalwareFamily { get; set; }

    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Optional Information for backward compatibility
    public OptionalInformationDto OptionalInformation { get; set; } = new();
}

/// <summary>
/// Backward compatibility DTO for existing API consumers
/// </summary>
public class OptionalInformationDto
{
    public string? DestinationAddress { get; set; }
    public string? DestinationCountry { get; set; }
    public string? DestinationPort { get; set; }
    public string? SourcePort { get; set; }
    public string? Protocol { get; set; }
    public string? Family { get; set; }
}