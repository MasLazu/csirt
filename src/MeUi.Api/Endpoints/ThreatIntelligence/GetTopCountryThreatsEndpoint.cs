using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatIntelligence.Queries.GetTopCountryThreats;

namespace MeUi.Api.Endpoints.ThreatIntelligence;

public class GetTopCountryThreatsEndpoint : BaseEndpoint<GetTopCountryThreatsQuery, TopCountryThreatsDto>
{
    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-intelligence/top-countries");
        AuthSchemes("Bearer");
        Description(x => x.WithTags("Threat Intelligence"));
        Summary(s =>
        {
            s.Summary = "Get top country threats";
            s.Description = @"
**Identify and rank countries by threat activity volume for geographic threat intelligence analysis.**

## Query Parameters:

### **Date Range**
- **startDate** (DateTime, optional): Start date for country threat analysis
  - Format: ISO 8601 (`2024-01-01T00:00:00Z`) or simple date (`2024-01-01`)
  - Example: `?startDate=2024-01-01`
  - Default: 7 days ago
  - **Use Case**: Define analysis period start

- **endDate** (DateTime, optional): End date for country threat analysis
  - Format: ISO 8601 (`2024-01-07T23:59:59Z`) or simple date (`2024-01-07`)
  - Example: `?endDate=2024-01-07`
  - Default: Current time
  - **Use Case**: Define analysis period end

### **Result Limiting**
- **limit** (int, optional): Maximum number of top countries to return
  - Range: `1-100`
  - Default: `10`
  - Example: `?limit=25`
  - **Use Case**: Control result set size for visualization

## Response Data Structure:

### **Country Threat Data**
Each country entry contains:
- **countryCode**: 2-letter ISO 3166-1 country code (e.g., `US`, `CN`, `RU`)
- **countryName**: Full country name (e.g., `United States`, `China`, `Russia`)
- **threatCount**: Total number of threats originating from this country
- Automatic ranking by threat count (highest first)

### **Summary Information**
- **startDate**: Analysis period start (UTC)
- **endDate**: Analysis period end (UTC)
- **limit**: Maximum countries requested
- **totalCountries**: Number of countries returned

## Common High-Threat Countries:

### **Typical Top Threat Sources**
- **United States (US)**: Often high due to large internet infrastructure
- **China (CN)**: Significant threat activity volume
- **Russia (RU)**: Known for various cyber threat activities
- **Germany (DE)**: Major internet hub with mixed traffic
- **Netherlands (NL)**: Hosting and infrastructure hub
- **France (FR)**: Large internet presence
- **United Kingdom (GB)**: Major financial and tech center
- **Brazil (BR)**: Growing internet infrastructure
- **India (IN)**: Large internet user base
- **Japan (JP)**: Advanced technology infrastructure

## Example Requests:

### Get top 10 threat countries for last month:
```
GET /api/v1/threat-intelligence/top-countries?startDate=2024-01-01&endDate=2024-01-31&limit=10
```

### Analyze top 25 countries for current week:
```
GET /api/v1/threat-intelligence/top-countries?startDate=2024-01-15&endDate=2024-01-22&limit=25
```

### Default top 10 countries (last 7 days):
```
GET /api/v1/threat-intelligence/top-countries
```

### Extended analysis - top 50 countries:
```
GET /api/v1/threat-intelligence/top-countries?limit=50&startDate=2024-01-01&endDate=2024-03-31
```

### Focus on recent activity (last 24 hours):
```
GET /api/v1/threat-intelligence/top-countries?startDate=2024-01-20T00:00:00Z&endDate=2024-01-21T00:00:00Z&limit=15
```

## Perfect For:

### **Geographic Visualizations**
- **World Maps**: Color-coded threat intensity by country
- **Choropleth Maps**: Country-based threat heat mapping
- **Geographic Dashboards**: Regional threat monitoring

### **Ranking Displays**
- **Leaderboard Tables**: Top threatening countries list
- **Bar Charts**: Horizontal country threat comparison
- **Flag Displays**: Visual country identification

### **Security Analysis**
- **Geopolitical Threat Assessment**: Country-based risk analysis
- **Network Security**: Geographic blocking decisions
- **Threat Intelligence**: Regional threat pattern identification
- **Incident Response**: Geographic threat correlation

### **Dashboard Components**
- **Top Threats Widget**: Quick country threat overview
- **Geographic Summary**: Regional threat distribution
- **Alert Systems**: Country-based threat notifications

## Visualization Libraries:
- **Leaflet.js**: Interactive world maps with country data
- **D3.js**: Custom geographic visualizations
- **AmCharts**: Professional geographic charts
- **Google Charts**: GeoChart for country visualization
- **Plotly.js**: Geographic plotting and mapping

## Use Cases:

### **Security Operations**
- **Geographic Blocking**: Identify countries for network filtering
- **Threat Monitoring**: Track regional threat patterns
- **Incident Analysis**: Correlate attacks with geographic origins

### **Business Intelligence**
- **Risk Assessment**: Geographic risk evaluation
- **Compliance**: Regional threat reporting requirements
- **Strategic Planning**: Geographic security investments

### **Threat Intelligence**
- **Pattern Recognition**: Identify geographic threat trends
- **Attribution Analysis**: Country-based threat attribution
- **Threat Landscape**: Regional threat environment assessment

## Data Interpretation:
- **High Counts**: May indicate large internet infrastructure, not necessarily malicious intent
- **Emerging Threats**: New countries appearing in top rankings
- **Seasonal Patterns**: Geographic threat variations over time
- **Infrastructure Correlation**: Hosting providers and threat volumes
";
        });
    }

    public override async Task HandleAsync(GetTopCountryThreatsQuery req, CancellationToken ct)
    {
        var result = await Mediator.Send(req, ct);
        await SendSuccessAsync(result, "Top country threats retrieved successfully", ct);
    }
}