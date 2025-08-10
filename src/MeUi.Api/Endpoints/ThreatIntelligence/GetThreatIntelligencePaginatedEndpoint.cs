using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatIntelligence.Queries.GetThreatIntelligencePaginated;

namespace MeUi.Api.Endpoints.ThreatIntelligence;

public class GetThreatIntelligencePaginatedEndpoint : BaseEndpoint<GetThreatIntelligencePaginatedQuery, PaginatedThreatIntelligenceDto>
{
    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-intelligence/paginated");
        AuthSchemes("Bearer");
        Description(x => x.WithTags("Threat Intelligence"));
        Summary(s =>
        {
            s.Summary = "Get paginated threat intelligence data";
            s.Description = @"
**Advanced paginated threat intelligence data with search capabilities and enhanced filtering.**

## Query Parameters:

### **Date Range**
- **startDate** (DateTime, optional): Start date for threat data
  - Format: ISO 8601 (`2024-01-01T00:00:00Z`) or simple date (`2024-01-01`)
  - Example: `?startDate=2024-01-01`
  - Default: No start date limit

- **endDate** (DateTime, optional): End date for threat data
  - Format: ISO 8601 (`2024-01-31T23:59:59Z`) or simple date (`2024-01-31`)
  - Example: `?endDate=2024-01-31`
  - Default: No end date limit

### **Search & Filtering**
- **searchTerm** (string, optional): Global search across multiple fields
  - Searches in: IP addresses, ASN descriptions, country names, categories
  - Example: `?searchTerm=malware` or `?searchTerm=192.168`
  - Case-insensitive partial matching

- **category** (string, optional): Filter by specific threat category
  - Common values: `malware`, `botnet`, `phishing`, `spam`, `scanning`, `bruteforce`, `ddos`
  - Example: `?category=phishing`
  - Exact match, case-insensitive

- **sourceCountry** (string, optional): Filter by source country
  - Format: 2-letter ISO 3166-1 country code
  - Examples: `US` (United States), `CN` (China), `RU` (Russia), `DE` (Germany)
  - Example: `?sourceCountry=CN`

- **asn** (string, optional): Filter by Autonomous System Number
  - Format: With or without 'AS' prefix
  - Examples: `AS15169` (Google), `16509` (Amazon), `AS13335` (Cloudflare)
  - Example: `?asn=AS15169`

### **Pagination Controls**
- **page** (int, optional): Page number (starts from 1)
  - Minimum: `1`
  - Example: `?page=3`
  - Default: `1`

- **pageSize** (int, optional): Records per page
  - Range: `1-100`
  - Recommended: `25` for UI tables, `50` for exports
  - Example: `?pageSize=25`
  - Default: `25`

### **Sorting Options**
- **sortBy** (string, optional): Field to sort results by
  - Options: `timestamp`, `sourceIp`, `category`, `asn`, `sourceCountry`, `severity`
  - Example: `?sortBy=severity`
  - Default: `timestamp`

- **sortDescending** (bool, optional): Sort direction
  - `true`: Newest/highest first (Z-A, 9-0)
  - `false`: Oldest/lowest first (A-Z, 0-9)
  - Example: `?sortDescending=true`
  - Default: `true`

## Response Features:
- **Severity Classification**: Automatic threat severity (Critical, High, Medium, Low)
- **Enhanced Pagination**: Includes `hasNextPage`, `hasPreviousPage`, `totalPages`
- **Rich Data**: Full country names, ASN descriptions, protocol information

## Example Requests:

### Search for malware threats:
```
GET /api/v1/threat-intelligence/paginated?searchTerm=malware&pageSize=50
```

### Get high-severity threats from last week:
```
GET /api/v1/threat-intelligence/paginated?startDate=2024-01-15&category=botnet&sortBy=severity
```

### Browse threats by country with pagination:
```
GET /api/v1/threat-intelligence/paginated?sourceCountry=RU&page=2&pageSize=25&sortBy=timestamp
```

### Search specific ASN threats:
```
GET /api/v1/threat-intelligence/paginated?asn=AS15169&sortBy=sourceIp&sortDescending=false
```
";
        });
    }

    public override async Task HandleAsync(GetThreatIntelligencePaginatedQuery req, CancellationToken ct)
    {
        var result = await Mediator.Send(req, ct);
        await SendSuccessAsync(result, "Paginated threat intelligence data retrieved successfully", ct);
    }
}