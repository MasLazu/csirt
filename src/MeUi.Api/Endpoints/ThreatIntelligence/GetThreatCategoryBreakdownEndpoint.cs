using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatIntelligence.Queries.GetThreatCategoryBreakdown;

namespace MeUi.Api.Endpoints.ThreatIntelligence;

public class GetThreatCategoryBreakdownEndpoint : BaseEndpoint<GetThreatCategoryBreakdownQuery, ThreatCategoryBreakdownDto>
{
    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-intelligence/categories");
        AuthSchemes("Bearer");
        Description(x => x.WithTags("Threat Intelligence"));
        Summary(s =>
        {
            s.Summary = "Get threat category breakdown";
            s.Description = @"
**Analyze threat distribution by category with detailed breakdown and statistics.**

## Query Parameters:

### **Date Range**
- **startDate** (DateTime, optional): Start date for category analysis
  - Format: ISO 8601 (`2024-01-01T00:00:00Z`) or simple date (`2024-01-01`)
  - Example: `?startDate=2024-01-01`
  - Default: 7 days ago
  - **Use Case**: Define analysis period start

- **endDate** (DateTime, optional): End date for category analysis
  - Format: ISO 8601 (`2024-01-07T23:59:59Z`) or simple date (`2024-01-07`)
  - Example: `?endDate=2024-01-07`
  - Default: Current time
  - **Use Case**: Define analysis period end

## Response Data Includes:

### **Category Breakdown**
Each category contains:
- **category**: Threat category name
- **threatCount**: Number of threats in this category
- Automatic percentage calculation based on total threats

### **Common Threat Categories**
- **malware**: Malicious software threats
- **botnet**: Botnet command & control traffic
- **phishing**: Phishing and social engineering attempts
- **spam**: Spam and unwanted communications
- **scanning**: Port scanning and reconnaissance
- **bruteforce**: Brute force authentication attempts
- **ddos**: Distributed denial of service attacks
- **ransomware**: Ransomware-related activity
- **cryptomining**: Cryptocurrency mining malware
- **trojan**: Trojan horse malware

### **Summary Statistics**
- **totalCategories**: Number of distinct categories found
- **totalThreats**: Sum of all threats across categories
- **startDate**: Analysis period start (UTC)
- **endDate**: Analysis period end (UTC)

## Example Requests:

### Get category breakdown for last 30 days:
```
GET /api/v1/threat-intelligence/categories?startDate=2024-01-01&endDate=2024-01-31
```

### Analyze current week's threat categories:
```
GET /api/v1/threat-intelligence/categories?startDate=2024-01-15&endDate=2024-01-22
```

### Default 7-day category analysis:
```
GET /api/v1/threat-intelligence/categories
```

### Year-to-date category trends:
```
GET /api/v1/threat-intelligence/categories?startDate=2024-01-01&endDate=2024-12-31
```

## Perfect For:

### **Pie Charts**
- Category distribution visualization
- Threat type proportions
- Security focus prioritization

### **Bar Charts**
- Category comparison
- Threat volume by type
- Trend analysis over time

### **Dashboard Widgets**
- Category summary cards
- Top threat types display
- Security metrics overview

### **Security Analysis**
- **Threat Landscape Assessment**: Understanding dominant threat types
- **Security Strategy Planning**: Focus areas identification
- **Resource Allocation**: Priority-based security investments
- **Trend Analysis**: Category evolution over time
- **Incident Response**: Category-specific response procedures

### **Reporting**
- Executive security summaries
- Threat intelligence reports
- Security posture assessments
- Compliance documentation

## Visualization Examples:

### **Donut Chart**: Category distribution with percentages
### **Horizontal Bar Chart**: Categories ranked by threat count
### **Treemap**: Hierarchical category visualization
### **Stacked Area Chart**: Category trends over time

## Use Cases:
- **SOC Dashboards**: Real-time threat category monitoring
- **Executive Reports**: High-level threat landscape overview
- **Threat Hunting**: Category-focused investigation
- **Security Planning**: Resource allocation by threat type
- **Compliance Reporting**: Threat category documentation
";
        });
    }

    public override async Task HandleAsync(GetThreatCategoryBreakdownQuery req, CancellationToken ct)
    {
        var result = await Mediator.Send(req, ct);
        await SendSuccessAsync(result, "Threat category breakdown retrieved successfully", ct);
    }
}