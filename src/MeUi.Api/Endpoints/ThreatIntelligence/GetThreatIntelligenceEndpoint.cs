using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatIntelligence.Queries.GetThreatIntelligence;

namespace MeUi.Api.Endpoints.ThreatIntelligence;

public class GetThreatIntelligenceEndpoint : BaseEndpoint<GetThreatIntelligenceQuery, ThreatIntelligenceListDto>
{
    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-intelligence");
        AuthSchemes("Bearer");
        Description(x => x.WithTags("Threat Intelligence"));
        Summary(s =>
        {
            s.Summary = "Get threat intelligence data";
            s.Description = @"
**Retrieve paginated threat intelligence data with comprehensive filtering and sorting options.**

## Query Parameters:

### **Date Filtering**
- **startDate** (DateTime, optional): Start date for filtering threats
  - Format: `2024-01-01T00:00:00Z` or `2024-01-01`
  - Example: `?startDate=2024-01-01T00:00:00Z`
  - Default: No start date filter

- **endDate** (DateTime, optional): End date for filtering threats
  - Format: `2024-01-31T23:59:59Z` or `2024-01-31`
  - Example: `?endDate=2024-01-31T23:59:59Z`
  - Default: No end date filter

### **Source Filtering**
- **category** (string, optional): Filter by threat category
  - Examples: `malware`, `botnet`, `phishing`, `spam`, `scanning`, `bruteforce`
  - Example: `?category=malware`
  - Case-insensitive matching

- **sourceCountry** (string, optional): Filter by source country code
  - Format: 2-letter ISO country code
  - Examples: `US`, `CN`, `RU`, `DE`, `GB`
  - Example: `?sourceCountry=US`

- **asn** (string, optional): Filter by Autonomous System Number
  - Format: ASN number with or without 'AS' prefix
  - Examples: `AS1234`, `1234`
  - Example: `?asn=AS1234`

- **sourceAddress** (string, optional): Filter by source IP address
  - Format: IPv4 or IPv6 address
  - Examples: `192.168.1.1`, `2001:db8::1`
  - Example: `?sourceAddress=192.168.1.1`

### **Pagination**
- **page** (int, optional): Page number (1-based)
  - Default: `1`
  - Example: `?page=2`

- **pageSize** (int, optional): Number of items per page
  - Range: 1-100
  - Default: `20`
  - Example: `?pageSize=50`

### **Sorting**
- **sortBy** (string, optional): Field to sort by
  - Options: `timestamp`, `sourceAddress`, `category`, `asn`, `sourceCountry`
  - Default: `timestamp`
  - Example: `?sortBy=category`

- **sortDescending** (bool, optional): Sort order
  - `true` for descending, `false` for ascending
  - Default: `true`
  - Example: `?sortDescending=false`

## Example Requests:

### Get recent malware threats from US:
```
GET /api/v1/threat-intelligence?category=malware&sourceCountry=US&startDate=2024-01-01
```

### Get threats from specific ASN, sorted by IP:
```
GET /api/v1/threat-intelligence?asn=AS1234&sortBy=sourceAddress&sortDescending=false
```

### Get paginated results with date range:
```
GET /api/v1/threat-intelligence?startDate=2024-01-01&endDate=2024-01-31&page=2&pageSize=50
```
";
        });
    }

    public override async Task HandleAsync(GetThreatIntelligenceQuery req, CancellationToken ct)
    {
        var result = await Mediator.Send(req, ct);
        await SendSuccessAsync(result, "Threat intelligence data retrieved successfully", ct);
    }
}