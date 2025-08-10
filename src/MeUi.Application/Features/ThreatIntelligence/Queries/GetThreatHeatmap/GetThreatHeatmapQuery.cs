using MediatR;

namespace MeUi.Application.Features.ThreatIntelligence.Queries.GetThreatHeatmap;

public record GetThreatHeatmapQuery : IRequest<ThreatHeatmapDto>
{
    public DateTime StartDate { get; init; } = DateTime.UtcNow.AddDays(-7);
    public DateTime EndDate { get; init; } = DateTime.UtcNow;
    public string HeatmapType { get; init; } = "geographic"; // "geographic", "temporal", "category"
    public int Resolution { get; init; } = 100; // Grid resolution for heatmap
}

public class ThreatHeatmapDto
{
    public string Type { get; set; } = string.Empty;
    public IEnumerable<HeatmapDataPointDto> DataPoints { get; set; } = new List<HeatmapDataPointDto>();
    public HeatmapMetadataDto Metadata { get; set; } = new HeatmapMetadataDto();
    public DateTime GeneratedAt { get; set; }
}

public class HeatmapDataPointDto
{
    public double X { get; set; } // Longitude for geographic, hour for temporal
    public double Y { get; set; } // Latitude for geographic, day for temporal
    public long Intensity { get; set; } // Threat count
    public string? Label { get; set; } // Country name, time label, etc.
    public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
}

public class HeatmapMetadataDto
{
    public long TotalThreats { get; set; }
    public long MaxIntensity { get; set; }
    public long MinIntensity { get; set; }
    public int DataPointCount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Resolution { get; set; } = string.Empty;
}