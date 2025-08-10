using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatIntelligence.Queries.GetThreatTimeSeries;

namespace MeUi.Api.Endpoints.ThreatIntelligence;

public class GetThreatTimeSeriesEndpoint : BaseEndpoint<GetThreatTimeSeriesNewQuery, ThreatTimeSeriesDto>
{
    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-intelligence/time-series");
        AuthSchemes("Bearer");
        Description(x => x.WithTags("Threat Intelligence"));
        Summary(s =>
        {
            s.Summary = "Get threat intelligence time series data";
            s.Description = @"
**Retrieve time-bucketed threat intelligence data for trend analysis and time-series visualization using optimized TimescaleDB functions.**

## Query Parameters:

### **Date Range**
- **startDate** (DateTime, optional): Start date for time series analysis
  - Format: ISO 8601 (`2024-01-01T00:00:00Z`) or simple date (`2024-01-01`)
  - Example: `?startDate=2024-01-01T00:00:00Z`
  - Default: 7 days ago
  - **Use Case**: Define the beginning of your analysis window

- **endDate** (DateTime, optional): End date for time series analysis
  - Format: ISO 8601 (`2024-01-07T23:59:59Z`) or simple date (`2024-01-07`)
  - Example: `?endDate=2024-01-07T23:59:59Z`
  - Default: Current time
  - **Use Case**: Define the end of your analysis window

### **Time Granularity**
- **interval** (string, optional): Time bucket interval for aggregation
  - **Options**:
    - `hour`: Hourly threat counts (24 data points per day)
    - `day`: Daily threat counts (1 data point per day)
  - Example: `?interval=hour`
  - Default: `hour`
  - **Use Case**: Choose granularity based on analysis needs

## Response Data Structure:

### **Time Series Data Points**
Each data point contains:
- **timestamp**: Exact time of the bucket (ISO 8601 format)
- **threatCount**: Number of threats in this time bucket
- **interval**: Bucket type (`hour` or `day`)

### **Metadata**
- **startDate**: Actual start date of analysis (UTC)
- **endDate**: Actual end date of analysis (UTC)
- **interval**: Time bucket interval used
- **totalDataPoints**: Number of time buckets returned

## Time Bucket Behavior:

### **Hourly Buckets** (`interval=hour`)
- **Bucket Size**: 1 hour
- **Timestamp**: Start of each hour (e.g., `2024-01-01T14:00:00Z`)
- **Data Points**: Up to 24 per day
- **Best For**: Detailed trend analysis, peak hour identification
- **Example Period**: 7 days = ~168 data points

### **Daily Buckets** (`interval=day`)
- **Bucket Size**: 1 day
- **Timestamp**: Start of each day (e.g., `2024-01-01T00:00:00Z`)
- **Data Points**: 1 per day
- **Best For**: Long-term trends, weekly/monthly patterns
- **Example Period**: 30 days = 30 data points

## Example Requests:

### Hourly threat trends for last 24 hours:
```
GET /api/v1/threat-intelligence/time-series?interval=hour&startDate=2024-01-01T00:00:00Z&endDate=2024-01-02T00:00:00Z
```

### Daily threat trends for last month:
```
GET /api/v1/threat-intelligence/time-series?interval=day&startDate=2024-01-01&endDate=2024-01-31
```

### Weekly hourly pattern analysis:
```
GET /api/v1/threat-intelligence/time-series?interval=hour&startDate=2024-01-15&endDate=2024-01-22
```

### Default 7-day hourly analysis:
```
GET /api/v1/threat-intelligence/time-series
```

### Long-term daily trend (90 days):
```
GET /api/v1/threat-intelligence/time-series?interval=day&startDate=2024-01-01&endDate=2024-03-31
```

## Perfect For:

### **Chart Visualizations**
- **Line Charts**: Trend analysis over time
- **Area Charts**: Volume visualization
- **Bar Charts**: Discrete time period comparison
- **Sparklines**: Compact trend indicators

### **Analysis Use Cases**
- **Peak Hour Identification**: Find busiest threat periods
- **Trend Analysis**: Identify increasing/decreasing patterns
- **Anomaly Detection**: Spot unusual activity spikes
- **Capacity Planning**: Understand threat volume patterns
- **Incident Correlation**: Match threats to time periods

### **Dashboard Components**
- **Real-time Charts**: Live threat trend monitoring
- **Historical Analysis**: Long-term pattern identification
- **Comparative Analysis**: Period-over-period comparison
- **Alert Thresholds**: Baseline establishment

## Visualization Libraries:
- **Chart.js**: Simple time-series line charts
- **D3.js**: Custom time-series visualizations
- **Plotly.js**: Interactive time-series plots
- **ApexCharts**: Modern time-series charts
- **Recharts**: React time-series components

## Time Zone Handling:
- All timestamps returned in UTC
- Input dates automatically converted to UTC
- Frontend should handle local time zone display
- Consistent time bucketing regardless of input timezone

## Performance Optimization:
- Uses TimescaleDB time_bucket() function for efficiency
- Optimized for large time ranges
- Automatic data point limiting for performance
- Indexed timestamp queries for fast response
";
        });
    }

    public override async Task HandleAsync(GetThreatTimeSeriesNewQuery req, CancellationToken ct)
    {
        var result = await Mediator.Send(req, ct);
        await SendSuccessAsync(result, "Threat intelligence time series data retrieved successfully", ct);
    }
}