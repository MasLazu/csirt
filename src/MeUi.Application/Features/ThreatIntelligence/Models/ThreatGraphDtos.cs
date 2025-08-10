namespace MeUi.Application.Features.ThreatIntelligence.Models;

/// <summary>
/// DTO for threat intelligence relationship graph
/// </summary>
public class ThreatRelationshipGraphDto
{
    public IEnumerable<GraphNodeDto> Nodes { get; set; } = new List<GraphNodeDto>();
    public IEnumerable<GraphEdgeDto> Edges { get; set; } = new List<GraphEdgeDto>();
    public GraphMetadataDto Metadata { get; set; } = new GraphMetadataDto();
}

/// <summary>
/// DTO for graph node representing a threat entity
/// </summary>
public class GraphNodeDto
{
    public string Id { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "asn", "country", "category", "ip", "malware"
    public long ThreatCount { get; set; }
    public double Size { get; set; } // For visualization sizing
    public string Color { get; set; } = string.Empty; // For visualization coloring
    public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
}

/// <summary>
/// DTO for graph edge representing relationship between threat entities
/// </summary>
public class GraphEdgeDto
{
    public string Id { get; set; } = string.Empty;
    public string SourceId { get; set; } = string.Empty;
    public string TargetId { get; set; } = string.Empty;
    public string RelationType { get; set; } = string.Empty; // "attacks", "originates_from", "targets", etc.
    public long Weight { get; set; } // Number of threat instances
    public double Thickness { get; set; } // For visualization
    public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
}

/// <summary>
/// DTO for graph metadata
/// </summary>
public class GraphMetadataDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalNodes { get; set; }
    public int TotalEdges { get; set; }
    public long TotalThreats { get; set; }
    public Dictionary<string, int> NodeTypeCount { get; set; } = new Dictionary<string, int>();
    public Dictionary<string, int> RelationshipTypeCount { get; set; } = new Dictionary<string, int>();
}

/// <summary>
/// DTO for threat relationship aggregation
/// </summary>
public class ThreatRelationshipDto
{
    public string SourceType { get; set; } = string.Empty;
    public string SourceId { get; set; } = string.Empty;
    public string SourceLabel { get; set; } = string.Empty;
    public string TargetType { get; set; } = string.Empty;
    public string TargetId { get; set; } = string.Empty;
    public string TargetLabel { get; set; } = string.Empty;
    public string RelationType { get; set; } = string.Empty;
    public long ThreatCount { get; set; }
}