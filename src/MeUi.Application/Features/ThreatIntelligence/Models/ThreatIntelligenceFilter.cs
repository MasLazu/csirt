using System.ComponentModel.DataAnnotations;

namespace MeUi.Application.Features.ThreatIntelligence.Models;

public class ThreatIntelligenceFilter
{
    /// <summary>
    /// Filter by Autonomous System Number (ASN)
    /// </summary>
    public string? Asn { get; set; }

    /// <summary>
    /// Filter by source IP address
    /// </summary>
    public string? SourceAddress { get; set; }

    /// <summary>
    /// Filter by destination IP address
    /// </summary>
    public string? DestinationAddress { get; set; }

    /// <summary>
    /// Filter by source country code
    /// </summary>
    [StringLength(2, MinimumLength = 2, ErrorMessage = "Source country must be a 2-character country code")]
    public string? SourceCountry { get; set; }

    /// <summary>
    /// Filter by destination country code
    /// </summary>
    [StringLength(2, MinimumLength = 2, ErrorMessage = "Destination country must be a 2-character country code")]
    public string? DestinationCountry { get; set; }

    /// <summary>
    /// Filter by threat category
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Filter by network protocol (TCP, UDP, etc.)
    /// </summary>
    public string? Protocol { get; set; }

    /// <summary>
    /// Filter by source port number
    /// </summary>
    [Range(1, 65535, ErrorMessage = "Source port must be between 1 and 65535")]
    public int? SourcePort { get; set; }

    /// <summary>
    /// Filter by destination port number
    /// </summary>
    [Range(1, 65535, ErrorMessage = "Destination port must be between 1 and 65535")]
    public int? DestinationPort { get; set; }

    /// <summary>
    /// Filter by malware family name
    /// </summary>
    public string? MalwareFamily { get; set; }

    /// <summary>
    /// Filter records from this date onwards (inclusive)
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Filter records up to this date (inclusive)
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Field to sort results by
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Sort in descending order (default: true)
    /// </summary>
    public bool SortDescending { get; set; } = true;

    /// <summary>
    /// Number of records to skip for pagination
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Skip must be a non-negative number")]
    public int Skip { get; set; } = 0;

    /// <summary>
    /// Number of records to take for pagination
    /// </summary>
    [Range(1, 1000, ErrorMessage = "Take must be between 1 and 1000")]
    public int Take { get; set; } = 10;
}