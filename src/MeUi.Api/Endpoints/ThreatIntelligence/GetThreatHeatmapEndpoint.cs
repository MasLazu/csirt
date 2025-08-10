using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatIntelligence.Queries.GetThreatHeatmap;

namespace MeUi.Api.Endpoints.ThreatIntelligence;

public class GetThreatHeatmapEndpoint : BaseEndpoint<GetThreatHeatmapQuery, ThreatHeatmapDto>
{
    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-intelligence/heatmap");
        AuthSchemes("Bearer");
        Description(x => x.WithTags("Threat Intelligence"));
        Summary(s =>
        {
            s.Summary = "Get threat intelligence heatmap";
            s.Description = @"
**Generate heatmap visualization data for threat intelligence analysis across geographic, temporal, or categorical dimensions.**

## Query Parameters:

### **Date Range**
- **startDate** (DateTime, optional): Start date for heatmap analysis
  - Format: ISO 8601 (`2024-01-01T00:00:00Z`) or simple date (`2024-01-01`)
  - Example: `?startDate=2024-01-01`
  - Default: 7 days ago
  - **Use Case**: Define analysis time window

- **endDate** (DateTime, optional): End date for heatmap analysis
  - Format: ISO 8601 (`2024-01-07T23:59:59Z`) or simple date (`2024-01-07`)
  - Example: `?endDate=2024-01-07`
  - Default: Current time
  - **Use Case**: Define analysis time window

### **Heatmap Configuration**
- **heatmapType** (string, optional): Type of heatmap to generate
  - **Options**:
    - `geographic`: World map with country-based threat intensity
    - `temporal`: Time-based heatmap showing threat patterns over hours/days
    - `category`: Category-based grid showing threat type distribution
  - Example: `?heatmapType=geographic`
  - Default: `geographic`
  - **Use Case**: Choose visualization type

- **resolution** (int, optional): Grid resolution for heatmap rendering
  - Range: `10-500`
  - Default: `100`
  - Example: `?resolution=200`
  - **Use Case**: Control heatmap detail level (higher = more detailed)

## Heatmap Types Explained:

### **Geographic Heatmap** (`heatmapType=geographic`)
- **Purpose**: Show threat intensity by country/region
- **Data Points**: Country coordinates with threat counts
- **Visualization**: World map with color-coded threat intensity
- **Use Case**: Identify geographic threat hotspots
- **Example**: `?heatmapType=geographic&startDate=2024-01-01&endDate=2024-01-31`

### **Temporal Heatmap** (`heatmapType=temporal`)
- **Purpose**: Show threat patterns over time
- **Data Points**: Hour-of-day vs day-of-period grid
- **Visualization**: Calendar-style heatmap with time patterns
- **Use Case**: Identify peak threat times and patterns
- **Example**: `?heatmapType=temporal&startDate=2024-01-01&endDate=2024-01-07`

### **Category Heatmap** (`heatmapType=category`)
- **Purpose**: Show threat category distribution
- **Data Points**: Category-based grid layout
- **Visualization**: Grid of threat categories with intensity
- **Use Case**: Understand threat type distribution
- **Example**: `?heatmapType=category&startDate=2024-01-01&endDate=2024-01-31`

## Response Data Structure:

### **Data Points**
- **X, Y coordinates**: Position in heatmap grid
- **Intensity**: Threat count for visualization scaling
- **Label**: Human-readable description
- **Properties**: Additional metadata for tooltips

### **Metadata**
- **Total threats**: Sum of all data points
- **Max/Min intensity**: For color scaling
- **Data point count**: Number of heatmap cells
- **Resolution info**: Grid dimensions

## Example Requests:

### World threat heatmap for last month:
```
GET /api/v1/threat-intelligence/heatmap?heatmapType=geographic&startDate=2024-01-01&endDate=2024-01-31
```

### Time pattern analysis for last week:
```
GET /api/v1/threat-intelligence/heatmap?heatmapType=temporal&startDate=2024-01-15&endDate=2024-01-22&resolution=168
```

### Category distribution heatmap:
```
GET /api/v1/threat-intelligence/heatmap?heatmapType=category&startDate=2024-01-01&endDate=2024-01-31
```

### High-resolution geographic heatmap:
```
GET /api/v1/threat-intelligence/heatmap?heatmapType=geographic&resolution=300&startDate=2024-01-01
```

## Perfect For:
- **Geographic Visualization**: World maps, regional analysis
- **Time Pattern Analysis**: Peak hours, seasonal trends
- **Category Analysis**: Threat type distribution
- **Heat Map Libraries**: D3.js, Leaflet, Chart.js heatmaps
- **Security Dashboards**: Visual threat landscape overview
- **Threat Intelligence Reports**: Visual threat summaries

## Visualization Libraries:
- **Leaflet.js**: Geographic heatmaps on world maps
- **D3.js**: Custom heatmap visualizations
- **Chart.js**: Time-based heatmap charts
- **Plotly.js**: Interactive heatmap plots
";
        });
    }

    public override async Task HandleAsync(GetThreatHeatmapQuery req, CancellationToken ct)
    {
        var result = await Mediator.Send(req, ct);
        await SendSuccessAsync(result, "Threat intelligence heatmap generated successfully", ct);
    }
}