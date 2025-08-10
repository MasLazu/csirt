using MediatR;
using MeUi.Application.Features.ThreatIntelligence.Models;

namespace MeUi.Application.Features.ThreatIntelligence.Queries.GetThreatRelationshipGraph;

public record GetThreatRelationshipGraphQuery : IRequest<ThreatRelationshipGraphDto>
{
    public DateTime StartDate { get; init; } = DateTime.UtcNow.AddDays(-7);
    public DateTime EndDate { get; init; } = DateTime.UtcNow;
    public int MaxNodes { get; init; } = 50;
    public int MinThreatCount { get; init; } = 1;
    public string RelationshipTypes { get; init; } = "asn-country,country-category";
}

public class ThreatRelationshipGraphDto
{
    public IEnumerable<GraphNodeDto> Nodes { get; set; } = new List<GraphNodeDto>();
    public IEnumerable<GraphEdgeDto> Edges { get; set; } = new List<GraphEdgeDto>();
    public int TotalNodes { get; set; }
    public int TotalEdges { get; set; }
}

public class GraphNodeDto
{
    public string Id { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public long ThreatCount { get; set; }
    public double Size { get; set; }
    public string Color { get; set; } = string.Empty;
}

public class GraphEdgeDto
{
    public string Id { get; set; } = string.Empty;
    public string SourceId { get; set; } = string.Empty;
    public string TargetId { get; set; } = string.Empty;
    public string RelationType { get; set; } = string.Empty;
    public long Weight { get; set; }
    public double Thickness { get; set; }
}