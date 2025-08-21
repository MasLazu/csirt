using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatIntelligentOverview;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace MeUi.Infrastructure.Data.Repositories;

public class TenantThreatIntelligentOverviewRepository : ITenantThreatIntelligentOverviewRepository
{
    private readonly string _connectionString;

    public TenantThreatIntelligentOverviewRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection string is not configured.");
    }

    private IDbConnection CreateConnection()
    {
        var connection = new NpgsqlConnection(_connectionString);
        connection.Open();
        return connection;
    }

    public async Task<List<ExecutiveSummaryMetricDto>> GetExecutiveSummaryAsync(Guid tenantId, DateTime start, DateTime end, CancellationToken cancellationToken = default)
    {
        string sql = @"
SELECT 'Total Events' as Metric, COUNT(*) as Count, 'All threat events in selected time range' as Description 
FROM ""ThreatEvents"" te 
WHERE te.""DeletedAt"" IS NULL 
    AND te.""Timestamp"" BETWEEN @start AND @end 
    AND te.""AsnRegistryId"" IN (SELECT ""AsnRegistryId"" FROM ""TenantAsnRegistries"" WHERE ""TenantId"" = @tenantId AND ""DeletedAt"" IS NULL)
UNION ALL
SELECT 'Active Categories' as Metric, COUNT(DISTINCT ""Category"") as Count, 'Distinct threat categories detected' as Description 
FROM ""ThreatEvents"" te 
WHERE te.""DeletedAt"" IS NULL 
    AND te.""Timestamp"" BETWEEN @start AND @end 
    AND te.""AsnRegistryId"" IN (SELECT ""AsnRegistryId"" FROM ""TenantAsnRegistries"" WHERE ""TenantId"" = @tenantId AND ""DeletedAt"" IS NULL)
UNION ALL
SELECT 'Unique Source IPs' as Metric, COUNT(DISTINCT ""SourceAddress"") as Count, 'Distinct attacking IP addresses' as Description 
FROM ""ThreatEvents"" te 
WHERE te.""DeletedAt"" IS NULL 
    AND te.""Timestamp"" BETWEEN @start AND @end 
    AND te.""AsnRegistryId"" IN (SELECT ""AsnRegistryId"" FROM ""TenantAsnRegistries"" WHERE ""TenantId"" = @tenantId AND ""DeletedAt"" IS NULL)
UNION ALL
SELECT 'Countries Involved' as Metric, COUNT(DISTINCT c.""Name"") as Count, 'Countries with threat activity' as Description 
FROM ""ThreatEvents"" te 
JOIN ""Countries"" c ON te.""SourceCountryId"" = c.""Id"" 
WHERE te.""DeletedAt"" IS NULL 
    AND te.""Timestamp"" BETWEEN @start AND @end 
    AND te.""AsnRegistryId"" IN (SELECT ""AsnRegistryId"" FROM ""TenantAsnRegistries"" WHERE ""TenantId"" = @tenantId AND ""DeletedAt"" IS NULL)
UNION ALL
SELECT 'ASN Networks' as Metric, COUNT(DISTINCT ar.""Number"") as Count, 'Autonomous System Networks involved' as Description 
FROM ""ThreatEvents"" te 
JOIN ""AsnRegistries"" ar ON te.""AsnRegistryId"" = ar.""Id"" 
WHERE te.""DeletedAt"" IS NULL 
    AND te.""Timestamp"" BETWEEN @start AND @end 
    AND te.""AsnRegistryId"" IN (SELECT ""AsnRegistryId"" FROM ""TenantAsnRegistries"" WHERE ""TenantId"" = @tenantId AND ""DeletedAt"" IS NULL)
UNION ALL
SELECT 'Protocols Used' as Metric, COUNT(DISTINCT p.""Name"") as Count, 'Network protocols in attacks' as Description 
FROM ""ThreatEvents"" te 
JOIN ""Protocols"" p ON te.""ProtocolId"" = p.""Id"" 
WHERE te.""DeletedAt"" IS NULL 
    AND te.""Timestamp"" BETWEEN @start AND @end 
    AND te.""AsnRegistryId"" IN (SELECT ""AsnRegistryId"" FROM ""TenantAsnRegistries"" WHERE ""TenantId"" = @tenantId AND ""DeletedAt"" IS NULL)
";
        using IDbConnection connection = CreateConnection();
        IEnumerable<ExecutiveSummaryMetricDto> result = await connection.QueryAsync<ExecutiveSummaryMetricDto>(sql, new { start, end, tenantId }, commandTimeout: 300);
        return result.AsList();
    }

    public async Task<List<TimelineDataPointDto>> GetThreatActivityTimelineAsync(Guid tenantId, DateTime start, DateTime end, TimeSpan interval, CancellationToken cancellationToken = default)
    {
        string intervalStr = interval.TotalDays >= 1 ? "day" : interval.TotalHours >= 1 ? "hour" : "minute";
        string sql = $@"
SELECT date_trunc('{intervalStr}', ""Timestamp"") as Time, COUNT(*) as ThreatEvents
FROM ""ThreatEvents"" te
WHERE te.""DeletedAt"" IS NULL 
    AND te.""Timestamp"" BETWEEN @start AND @end
    AND te.""AsnRegistryId"" IN (SELECT ""AsnRegistryId"" FROM ""TenantAsnRegistries"" WHERE ""TenantId"" = @tenantId AND ""DeletedAt"" IS NULL)
GROUP BY Time
ORDER BY Time
";
        using IDbConnection connection = CreateConnection();
        IEnumerable<TimelineDataPointDto> result = await connection.QueryAsync<TimelineDataPointDto>(sql, new { start, end, tenantId }, commandTimeout: 300);
        return result.AsList();
    }
 
   public async Task<List<TopCategoryDto>> GetTopThreatCategoriesAsync(Guid tenantId, DateTime start, DateTime end, int limit = 5, CancellationToken cancellationToken = default)
    {
        string sql = @"
SELECT ""Category"", COUNT(*) as Events
FROM ""ThreatEvents"" te
WHERE te.""DeletedAt"" IS NULL 
    AND te.""Timestamp"" BETWEEN @start AND @end
    AND te.""AsnRegistryId"" IN (SELECT ""AsnRegistryId"" FROM ""TenantAsnRegistries"" WHERE ""TenantId"" = @tenantId AND ""DeletedAt"" IS NULL)
GROUP BY ""Category""
ORDER BY Events DESC
LIMIT @limit
";
        using IDbConnection connection = CreateConnection();
        IEnumerable<TopCategoryDto> result = await connection.QueryAsync<TopCategoryDto>(sql, new { start, end, tenantId, limit }, commandTimeout: 300);
        return result.AsList();
    }

    public async Task<List<TopCountryDto>> GetTopSourceCountriesAsync(Guid tenantId, DateTime start, DateTime end, int limit = 10, CancellationToken cancellationToken = default)
    {
        string sql = @"
SELECT c.""Name"" as Country, COUNT(*) as Events
FROM ""ThreatEvents"" te
JOIN ""Countries"" c ON te.""SourceCountryId"" = c.""Id""
WHERE te.""DeletedAt"" IS NULL 
    AND te.""Timestamp"" BETWEEN @start AND @end
    AND te.""AsnRegistryId"" IN (SELECT ""AsnRegistryId"" FROM ""TenantAsnRegistries"" WHERE ""TenantId"" = @tenantId AND ""DeletedAt"" IS NULL)
GROUP BY c.""Name""
ORDER BY Events DESC
LIMIT @limit
";
        using IDbConnection connection = CreateConnection();
        IEnumerable<TopCountryDto> result = await connection.QueryAsync<TopCountryDto>(sql, new { start, end, tenantId, limit }, commandTimeout: 300);
        return result.AsList();
    }

    public async Task<List<ProtocolDistributionDto>> GetProtocolDistributionAsync(Guid tenantId, DateTime start, DateTime end, CancellationToken cancellationToken = default)
    {
        string sql = @"
SELECT p.""Name"" as Protocol, COUNT(*) as Events
FROM ""ThreatEvents"" te
JOIN ""Protocols"" p ON te.""ProtocolId"" = p.""Id""
WHERE te.""DeletedAt"" IS NULL 
    AND te.""Timestamp"" BETWEEN @start AND @end
    AND te.""AsnRegistryId"" IN (SELECT ""AsnRegistryId"" FROM ""TenantAsnRegistries"" WHERE ""TenantId"" = @tenantId AND ""DeletedAt"" IS NULL)
GROUP BY p.""Name""
ORDER BY Events DESC
";
        using IDbConnection connection = CreateConnection();
        IEnumerable<ProtocolDistributionDto> result = await connection.QueryAsync<ProtocolDistributionDto>(sql, new { start, end, tenantId }, commandTimeout: 300);
        return result.AsList();
    }

    public async Task<List<HighRiskSourceIpDto>> GetHighRiskSourceIpsAsync(Guid tenantId, DateTime start, DateTime end, int limit = 15, CancellationToken cancellationToken = default)
    {
        string sql = @"
SELECT CAST(""SourceAddress"" as TEXT) as SourceIp, COUNT(*) as Events, COUNT(DISTINCT ""Category"") as Categories, MAX(""Timestamp"") as LastSeen
FROM ""ThreatEvents"" te
WHERE te.""DeletedAt"" IS NULL 
    AND te.""Timestamp"" BETWEEN @start AND @end
    AND te.""AsnRegistryId"" IN (SELECT ""AsnRegistryId"" FROM ""TenantAsnRegistries"" WHERE ""TenantId"" = @tenantId AND ""DeletedAt"" IS NULL)
GROUP BY ""SourceAddress""
ORDER BY Events DESC
LIMIT @limit
";
        using IDbConnection connection = CreateConnection();
        IEnumerable<HighRiskSourceIpDto> result = await connection.QueryAsync<HighRiskSourceIpDto>(sql, new { start, end, tenantId, limit }, commandTimeout: 300);
        return result.AsList();
    }

    public async Task<List<TargetedPortDto>> GetTopTargetedPortsAsync(Guid tenantId, DateTime start, DateTime end, int limit = 10, CancellationToken cancellationToken = default)
    {
        string sql = @"
SELECT CAST(""DestinationPort"" as TEXT) as Port, COUNT(*) as Events
FROM ""ThreatEvents"" te
WHERE te.""DeletedAt"" IS NULL 
    AND te.""Timestamp"" BETWEEN @start AND @end 
    AND ""DestinationPort"" IS NOT NULL
    AND te.""AsnRegistryId"" IN (SELECT ""AsnRegistryId"" FROM ""TenantAsnRegistries"" WHERE ""TenantId"" = @tenantId AND ""DeletedAt"" IS NULL)
GROUP BY ""DestinationPort""
ORDER BY Events DESC
LIMIT @limit
";
        using IDbConnection connection = CreateConnection();
        IEnumerable<TargetedPortDto> result = await connection.QueryAsync<TargetedPortDto>(sql, new { start, end, tenantId, limit }, commandTimeout: 300);
        return result.AsList();
    }

    public async Task<List<ThreatCategoryAnalysisDto>> GetThreatCategoryAnalysisAsync(Guid tenantId, DateTime start, DateTime end, CancellationToken cancellationToken = default)
    {
        string sql = @"
SELECT ""Category"", COUNT(*) as TotalEvents, COUNT(DISTINCT ""SourceAddress"") as UniqueIps, COUNT(DISTINCT te.""SourceCountryId"") as Countries, MIN(""Timestamp"") as FirstSeen, MAX(""Timestamp"") as LastSeen
FROM ""ThreatEvents"" te
WHERE te.""DeletedAt"" IS NULL 
    AND te.""Timestamp"" BETWEEN @start AND @end
    AND te.""AsnRegistryId"" IN (SELECT ""AsnRegistryId"" FROM ""TenantAsnRegistries"" WHERE ""TenantId"" = @tenantId AND ""DeletedAt"" IS NULL)
GROUP BY ""Category""
ORDER BY TotalEvents DESC
";
        using IDbConnection connection = CreateConnection();
        IEnumerable<ThreatCategoryAnalysisDto> result = await connection.QueryAsync<ThreatCategoryAnalysisDto>(sql, new { start, end, tenantId }, commandTimeout: 300);
        return result.AsList();
    }
}