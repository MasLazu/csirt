using MediatR;

namespace MeUi.Application.Features.ThreatIntelligence.Queries.GetRealTimeThreats;

public record GetRealTimeThreatsQuery : IRequest<RealTimeThreatsDto>
{
    public int Limit { get; init; } = 50;
    public string? Category { get; init; }
    public string? Severity { get; init; }
}

public class RealTimeThreatsDto
{
    public IEnumerable<RealTimeThreatDto> Threats { get; set; } = new List<RealTimeThreatDto>();
    public int TotalCount { get; set; }
    public DateTime LastUpdated { get; set; }
    public RealTimeStatsDto Stats { get; set; } = new RealTimeStatsDto();
}

public class RealTimeThreatDto
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string SourceIp { get; set; } = string.Empty;
    public string? DestinationIp { get; set; }
    public string SourceCountry { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Asn { get; set; } = string.Empty;
    public int? SourcePort { get; set; }
    public int? DestinationPort { get; set; }
    public string? Protocol { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

public class RealTimeStatsDto
{
    public long ThreatsLastHour { get; set; }
    public long ThreatsLastMinute { get; set; }
    public double AverageThreatsPerMinute { get; set; }
    public string MostActiveCountry { get; set; } = string.Empty;
    public string MostCommonCategory { get; set; } = string.Empty;
    public int ActiveSources { get; set; }
}