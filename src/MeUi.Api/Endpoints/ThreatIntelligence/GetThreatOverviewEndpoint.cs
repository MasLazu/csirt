using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatIntelligence.Queries.GetThreatOverview;

namespace MeUi.Api.Endpoints.ThreatIntelligence;

public class GetThreatOverviewEndpoint : BaseEndpoint<GetThreatOverviewQuery, ThreatOverviewDto>
{
    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-intelligence/overview");
        AuthSchemes("Bearer");
        Description(x => x.WithTags("Threat Intelligence"));
        Summary(s =>
        {
            s.Summary = "Get threat intelligence overview";
            s.Description = @"
**Comprehensive threat intelligence dashboard overview with key metrics, trends, and geographic distribution.**

## Query Parameters:

### **Date Range**
- **startDate** (DateTime, optional): Start date for overview analysis
  - Format: ISO 8601 (`2024-01-01T00:00:00Z`) or simple date (`2024-01-01`)
  - Example: `?startDate=2024-01-01`
  - Default: 30 days ago
  - **Use Case**: Set analysis period start

- **endDate** (DateTime, optional): End date for overview analysis
  - Format: ISO 8601 (`2024-01-31T23:59:59Z`) or simple date (`2024-01-31`)
  - Example: `?endDate=2024-01-31`
  - Default: Current time
  - **Use Case**: Set analysis period end

## Response Data Includes:

### **Key Metrics**
- Total threats in period
- Threats today/this week/this month
- Threat growth rate percentage
- Unique source IPs count
- Unique ASNs count
- Affected countries count
- Threat categories count

### **Trend Analysis**
- Daily threat counts over the period
- Time-series data for charting
- Growth patterns and anomalies

### **Top Threats**
- Top threatening countries
- Most active ASNs
- Common threat categories
- Severity classifications

### **Geographic Distribution**
- Country-wise threat distribution
- Latitude/longitude coordinates for mapping
- Percentage breakdown by region

## Example Requests:

### Get last 7 days overview:
```
GET /api/v1/threat-intelligence/overview?startDate=2024-01-15&endDate=2024-01-22
```

### Get monthly overview:
```
GET /api/v1/threat-intelligence/overview?startDate=2024-01-01&endDate=2024-01-31
```

### Get current month overview (default):
```
GET /api/v1/threat-intelligence/overview
```

## Perfect For:
- **Executive Dashboards**: High-level KPIs and metrics
- **Security Operations Centers**: Real-time threat landscape
- **Trend Analysis**: Historical threat patterns
- **Geographic Visualization**: World map threat overlays
- **Reporting**: Automated threat intelligence reports
";
        });
    }

    public override async Task HandleAsync(GetThreatOverviewQuery req, CancellationToken ct)
    {
        var result = await Mediator.Send(req, ct);
        await SendSuccessAsync(result, "Threat intelligence overview retrieved successfully", ct);
    }
}