using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatIntelligence.Queries.GetThreatRelationshipGraph;

namespace MeUi.Api.Endpoints.ThreatIntelligence;

public class GetThreatRelationshipGraphEndpoint : BaseEndpoint<GetThreatRelationshipGraphQuery, ThreatRelationshipGraphDto>
{
    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-intelligence/relationship-graph");
        AuthSchemes("Bearer");
        Description(x => x.WithTags("Threat Intelligence"));
        Summary(s =>
        {
            s.Summary = "Get threat intelligence relationship graph";
            s.Description = @"
**Generate network graph data showing relationships between threat intelligence entities for interactive visualization.**

## Query Parameters:

### **Date Range**
- **startDate** (DateTime, optional): Start date for relationship analysis
  - Format: ISO 8601 (`2024-01-01T00:00:00Z`) or simple date (`2024-01-01`)
  - Example: `?startDate=2024-01-01`
  - Default: 7 days ago
  - **Use Case**: Define analysis time window

- **endDate** (DateTime, optional): End date for relationship analysis
  - Format: ISO 8601 (`2024-01-07T23:59:59Z`) or simple date (`2024-01-07`)
  - Example: `?endDate=2024-01-07`
  - Default: Current time
  - **Use Case**: Define analysis time window

### **Graph Configuration**
- **maxNodes** (int, optional): Maximum number of nodes in the graph
  - Range: `10-200`
  - Default: `50`
  - Example: `?maxNodes=100`
  - **Use Case**: Control graph complexity and performance

- **minThreatCount** (int, optional): Minimum threat count threshold for inclusion
  - Range: `1-1000`
  - Default: `1`
  - Example: `?minThreatCount=10`
  - **Use Case**: Filter out low-activity entities

### **Relationship Types**
- **relationshipTypes** (string, optional): Comma-separated list of relationship types to include
  - **Available Types**:
    - `asn-country`: ASN to Country connections
    - `country-category`: Country to Threat Category connections
    - `asn-category`: ASN to Threat Category connections
    - `ip-asn`: IP Address to ASN connections
    - `malware-category`: Malware Family to Category connections
  - Example: `?relationshipTypes=asn-country,country-category`
  - Default: `asn-country,country-category`
  - **Use Case**: Focus on specific relationship types

## Response Data Structure:

### **Nodes** (Graph Entities)
Each node represents a threat intelligence entity:
- **id**: Unique identifier (e.g., `asn:AS1234`, `country:US`)
- **label**: Human-readable name
- **type**: Entity type (`asn`, `country`, `category`, `ip`, `malware`)
- **threatCount**: Total threats associated with this entity
- **size**: Visual size for rendering (10-50 pixels)
- **color**: Hex color code for visualization

### **Edges** (Relationships)
Each edge represents a relationship between entities:
- **id**: Unique edge identifier
- **sourceId**: Source node ID
- **targetId**: Target node ID
- **relationType**: Type of relationship
- **weight**: Strength of relationship (threat count)
- **thickness**: Visual thickness for rendering (1-8 pixels)

### **Node Types & Colors**
- **ASN** (`asn`): Red (#FF6B6B) - Autonomous System Numbers
- **Country** (`country`): Teal (#4ECDC4) - Geographic locations
- **Category** (`category`): Blue (#45B7D1) - Threat categories
- **IP** (`ip`): Green (#96CEB4) - IP addresses
- **Malware** (`malware`): Yellow (#FFEAA7) - Malware families

### **Relationship Types**
- **originates_from**: ASN originates from Country
- **generates**: Country generates Category threats
- **produces**: ASN produces Category threats
- **belongs_to**: IP belongs to ASN
- **classified_as**: Malware classified as Category

## Example Requests:

### Basic ASN-Country relationship graph:
```
GET /api/v1/threat-intelligence/relationship-graph?relationshipTypes=asn-country&maxNodes=30
```

### Multi-relationship analysis:
```
GET /api/v1/threat-intelligence/relationship-graph?relationshipTypes=asn-country,country-category,malware-category&maxNodes=75
```

### High-activity entities only:
```
GET /api/v1/threat-intelligence/relationship-graph?minThreatCount=50&maxNodes=40
```

### Comprehensive relationship analysis:
```
GET /api/v1/threat-intelligence/relationship-graph?startDate=2024-01-01&endDate=2024-01-31&relationshipTypes=asn-country,country-category,asn-category&maxNodes=100&minThreatCount=10
```

### Focus on specific time period:
```
GET /api/v1/threat-intelligence/relationship-graph?startDate=2024-01-15&endDate=2024-01-22&relationshipTypes=asn-country,ip-asn
```

## Perfect For:
- **Network Diagrams**: Interactive threat relationship visualization
- **Threat Correlation**: Understanding entity connections
- **Attack Path Analysis**: Tracing threat propagation
- **Intelligence Analysis**: Identifying threat patterns
- **Interactive Dashboards**: Dynamic graph exploration
- **Threat Hunting**: Discovering related entities

## Visualization Libraries:
- **D3.js**: Custom force-directed graphs
- **Vis.js**: Interactive network diagrams
- **Cytoscape.js**: Advanced graph visualization
- **Sigma.js**: High-performance graph rendering
- **React Flow**: React-based graph components

## Graph Algorithms:
- **Force-directed layout**: Natural node positioning
- **Hierarchical layout**: Structured relationship display
- **Circular layout**: Organized entity arrangement
- **Community detection**: Grouping related entities

## Use Cases:
- **SOC Analysis**: Understanding threat landscapes
- **Incident Response**: Tracing attack vectors
- **Threat Intelligence**: Relationship discovery
- **Security Research**: Pattern identification
- **Risk Assessment**: Entity impact analysis
";
        });
    }

    public override async Task HandleAsync(GetThreatRelationshipGraphQuery req, CancellationToken ct)
    {
        var result = await Mediator.Send(req, ct);
        await SendSuccessAsync(result, "Threat intelligence relationship graph retrieved successfully", ct);
    }
}