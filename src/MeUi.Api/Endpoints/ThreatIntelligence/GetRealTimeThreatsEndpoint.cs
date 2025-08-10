using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatIntelligence.Queries.GetRealTimeThreats;

namespace MeUi.Api.Endpoints.ThreatIntelligence;

public class GetRealTimeThreatsEndpoint : BaseEndpoint<GetRealTimeThreatsQuery, RealTimeThreatsDto>
{
    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-intelligence/real-time");
        AuthSchemes("Bearer");
        Description(x => x.WithTags("Threat Intelligence"));
        Summary(s =>
        {
            s.Summary = "Get real-time threat intelligence";
            s.Description = @"
**Real-time threat intelligence monitoring with live statistics and active threat detection.**

## Query Parameters:

### **Result Limiting**
- **limit** (int, optional): Maximum number of recent threats to return
  - Range: `1-200`
  - Default: `50`
  - Example: `?limit=100`
  - **Use Case**: Control data volume for real-time displays

### **Threat Filtering**
- **category** (string, optional): Filter by specific threat category
  - Common values: `malware`, `botnet`, `phishing`, `spam`, `scanning`, `bruteforce`, `ddos`, `ransomware`
  - Example: `?category=malware`
  - **Use Case**: Focus on specific threat types

- **severity** (string, optional): Filter by threat severity level
  - Values: `Critical`, `High`, `Medium`, `Low`
  - Example: `?severity=Critical`
  - **Use Case**: Show only high-priority threats

## Response Data Includes:

### **Live Threat Feed**
- Most recent threats (last hour)
- Real-time threat details:
  - Source IP and country
  - Threat category and severity
  - ASN information
  - Port and protocol data
  - Descriptive threat summaries
  - Active status (threats in last 5 minutes)

### **Real-Time Statistics**
- **Threats last hour**: Total count in past 60 minutes
- **Threats last minute**: Very recent activity
- **Average per minute**: Threat velocity
- **Most active country**: Top source country
- **Most common category**: Dominant threat type
- **Active sources**: Estimated unique threat sources

### **Activity Indicators**
- **isActive**: Boolean flag for threats in last 5 minutes
- **Timestamp**: Exact time of threat detection
- **Description**: Human-readable threat summary

## Example Requests:

### Get latest 25 critical threats:
```
GET /api/v1/threat-intelligence/real-time?limit=25&severity=Critical
```

### Monitor malware activity:
```
GET /api/v1/threat-intelligence/real-time?category=malware&limit=100
```

### Get all recent threats (default):
```
GET /api/v1/threat-intelligence/real-time
```

### Focus on botnet activity:
```
GET /api/v1/threat-intelligence/real-time?category=botnet&severity=High&limit=75
```

## Perfect For:
- **Security Operations Centers (SOC)**: Live threat monitoring
- **Incident Response**: Real-time threat detection
- **Threat Hunting**: Active threat identification
- **Alert Systems**: High-priority threat notifications
- **Live Dashboards**: Real-time security status displays
- **Automated Response**: Trigger-based security actions

## Update Frequency:
- Data refreshed every minute
- Statistics calculated in real-time
- Active status updated every 5 minutes
";
        });
    }

    public override async Task HandleAsync(GetRealTimeThreatsQuery req, CancellationToken ct)
    {
        var result = await Mediator.Send(req, ct);
        await SendSuccessAsync(result, "Real-time threat intelligence retrieved successfully", ct);
    }
}