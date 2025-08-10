using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatIntelligence.Queries.GetTopAsnThreats;

namespace MeUi.Api.Endpoints.ThreatIntelligence;

public class GetTopAsnThreatsEndpoint : BaseEndpoint<GetTopAsnThreatsQuery, TopAsnThreatsDto>
{
    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-intelligence/top-asns");
        AuthSchemes("Bearer");
        Description(x => x.WithTags("Threat Intelligence"));
        Summary(s =>
        {
            s.Summary = "Get top ASN threats";
            s.Description = @"
**Identify and rank Autonomous System Numbers (ASNs) by threat activity for network-level threat intelligence analysis.**

## Query Parameters:

### **Date Range**
- **startDate** (DateTime, optional): Start date for ASN threat analysis
  - Format: ISO 8601 (`2024-01-01T00:00:00Z`) or simple date (`2024-01-01`)
  - Example: `?startDate=2024-01-01`
  - Default: 7 days ago
  - **Use Case**: Define analysis period start

- **endDate** (DateTime, optional): End date for ASN threat analysis
  - Format: ISO 8601 (`2024-01-07T23:59:59Z`) or simple date (`2024-01-07`)
  - Example: `?endDate=2024-01-07`
  - Default: Current time
  - **Use Case**: Define analysis period end

### **Result Limiting**
- **limit** (int, optional): Maximum number of top ASNs to return
  - Range: `1-100`
  - Default: `10`
  - Example: `?limit=25`
  - **Use Case**: Control result set size for analysis

## Response Data Structure:

### **ASN Threat Data**
Each ASN entry contains:
- **asn**: ASN number (e.g., `AS15169`, `AS16509`, `AS13335`)
- **asnDescription**: Organization name and description
- **threatCount**: Total number of threats from this ASN
- Automatic ranking by threat count (highest first)

### **Summary Information**
- **startDate**: Analysis period start (UTC)
- **endDate**: Analysis period end (UTC)
- **limit**: Maximum ASNs requested
- **totalAsns**: Number of ASNs returned

## Understanding ASNs:

### **What is an ASN?**
- **Autonomous System Number**: Unique identifier for network operators
- **Network Ownership**: Identifies who controls IP address blocks
- **Routing Information**: Used for internet routing decisions
- **Organization Mapping**: Links IP addresses to organizations

### **Common High-Volume ASNs**
- **AS15169 (Google LLC)**: Google's infrastructure and services
- **AS16509 (Amazon.com)**: Amazon Web Services (AWS)
- **AS13335 (Cloudflare)**: Cloudflare CDN and security services
- **AS8075 (Microsoft)**: Microsoft Azure and services
- **AS32934 (Facebook)**: Meta/Facebook infrastructure
- **AS14061 (DigitalOcean)**: DigitalOcean hosting services
- **AS20940 (Akamai)**: Akamai CDN and cloud services
- **AS36459 (GitHub)**: GitHub hosting and services

### **ASN Categories**
- **Cloud Providers**: AWS, Azure, Google Cloud
- **CDN Services**: Cloudflare, Akamai, Fastly
- **Hosting Providers**: DigitalOcean, Linode, Vultr
- **ISPs**: Comcast, Verizon, AT&T
- **Enterprise**: Large corporations with their own ASNs

## Example Requests:

### Get top 10 ASNs for last month:
```
GET /api/v1/threat-intelligence/top-asns?startDate=2024-01-01&endDate=2024-01-31&limit=10
```

### Analyze top 25 ASNs for current week:
```
GET /api/v1/threat-intelligence/top-asns?startDate=2024-01-15&endDate=2024-01-22&limit=25
```

### Default top 10 ASNs (last 7 days):
```
GET /api/v1/threat-intelligence/top-asns
```

### Extended analysis - top 50 ASNs:
```
GET /api/v1/threat-intelligence/top-asns?limit=50&startDate=2024-01-01&endDate=2024-03-31
```

### Recent activity focus (last 48 hours):
```
GET /api/v1/threat-intelligence/top-asns?startDate=2024-01-19T00:00:00Z&endDate=2024-01-21T00:00:00Z&limit=20
```

## Perfect For:

### **Network Security Analysis**
- **ASN-based Blocking**: Identify problematic network operators
- **Traffic Analysis**: Understand threat sources by network
- **Incident Response**: Correlate attacks with network operators

### **Threat Intelligence**
- **Attribution Analysis**: Link threats to network infrastructure
- **Pattern Recognition**: Identify ASN-based threat patterns
- **Infrastructure Mapping**: Understand threat actor infrastructure

### **Visualizations**
- **Bar Charts**: ASN threat volume comparison
- **Network Diagrams**: ASN relationship mapping
- **Ranking Tables**: Top threatening ASNs list
- **Pie Charts**: ASN threat distribution

### **Security Operations**
- **Network Monitoring**: ASN-based threat tracking
- **Firewall Rules**: ASN-based filtering decisions
- **Threat Hunting**: ASN-focused investigation

## Data Interpretation:

### **High Threat Counts May Indicate**
- **Large Infrastructure**: Major cloud/hosting providers
- **Compromised Resources**: Hijacked or abused services
- **Legitimate Traffic**: High-volume legitimate services
- **Attack Infrastructure**: Dedicated threat actor resources

### **Analysis Considerations**
- **Context Matters**: High counts don't always mean malicious ASN
- **Legitimate Services**: Cloud providers may have high counts
- **Compromised Infrastructure**: Legitimate ASNs can be abused
- **Threat Actor Preference**: Some ASNs favored for malicious activity

## Use Cases:

### **Security Operations Center (SOC)**
- **Real-time Monitoring**: Track ASN-based threat activity
- **Alert Generation**: ASN-based threat notifications
- **Incident Analysis**: ASN correlation in security incidents

### **Network Security**
- **Access Control**: ASN-based network filtering
- **Risk Assessment**: ASN-based risk evaluation
- **Threat Prevention**: Proactive ASN blocking

### **Threat Intelligence**
- **Infrastructure Analysis**: Threat actor infrastructure mapping
- **Campaign Tracking**: ASN patterns in threat campaigns
- **Attribution**: Network-based threat attribution

## Visualization Libraries:
- **D3.js**: Custom ASN network visualizations
- **Chart.js**: ASN threat volume charts
- **Vis.js**: ASN relationship networks
- **Plotly.js**: Interactive ASN analysis plots
";
        });
    }

    public override async Task HandleAsync(GetTopAsnThreatsQuery req, CancellationToken ct)
    {
        var result = await Mediator.Send(req, ct);
        await SendSuccessAsync(result, "Top ASN threats retrieved successfully", ct);
    }
}