using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatGeographic;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace MeUi.Infrastructure.Data.Repositories;

public class TenantThreatGeographicRepository : ITenantThreatGeographicRepository
{
    private readonly string _connectionString;

    public TenantThreatGeographicRepository(IConfiguration configuration)
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

    public async Task<List<GeographicTrendDto>> GetGeographicTrendsAsync(Guid tenantId, DateTime start, DateTime end, TimeSpan interval, CancellationToken cancellationToken = default)
    {
        var intervalStr = interval.TotalDays >= 1 ? "day" : interval.TotalHours >= 1 ? "hour" : "minute";
        var sql = $@"
SELECT 
  date_trunc('{intervalStr}', te.""Timestamp"") as Time,
  c.""Name"" as Country,
  COUNT(*) as Events
FROM ""ThreatEvents"" te
JOIN ""Countries"" c ON te.""SourceCountryId"" = c.""Id""
WHERE te.""DeletedAt"" IS NULL 
  AND te.""Timestamp"" BETWEEN @start AND @end
  AND te.""AsnRegistryId"" IN (SELECT ""AsnRegistryId"" FROM ""TenantAsnRegistries"" WHERE ""TenantId"" = @tenantId AND ""DeletedAt"" IS NULL)
GROUP BY Time, c.""Name""
HAVING COUNT(*) > 5
ORDER BY Time";

        using var connection = CreateConnection();
        var result = await connection.QueryAsync<GeographicTrendDto>(sql, new { start, end, tenantId }, commandTimeout: 300);
        return result.AsList();
    }

    public async Task<List<CountryRankingDto>> GetCountryRankingsAsync(Guid tenantId, DateTime start, DateTime end, int limit = 20, CancellationToken cancellationToken = default)
    {
        var sql = @"
SELECT 
  c.""Name"" as Country,
  COUNT(*) as Events
FROM ""ThreatEvents"" te
JOIN ""Countries"" c ON te.""SourceCountryId"" = c.""Id""
WHERE te.""DeletedAt"" IS NULL 
  AND te.""Timestamp"" BETWEEN @start AND @end
  AND te.""AsnRegistryId"" IN (SELECT ""AsnRegistryId"" FROM ""TenantAsnRegistries"" WHERE ""TenantId"" = @tenantId AND ""DeletedAt"" IS NULL)
GROUP BY c.""Name""
ORDER BY Events DESC
LIMIT @limit";

        using var connection = CreateConnection();
        var result = await connection.QueryAsync<CountryRankingDto>(sql, new { start, end, tenantId, limit }, commandTimeout: 300);
        return result.AsList();
    }

    public async Task<List<CountryAsnCorrelationDto>> GetCountryAsnCorrelationAsync(Guid tenantId, DateTime start, DateTime end, int limit = 25, CancellationToken cancellationToken = default)
    {
        var sql = @"
SELECT 
  c.""Name"" as Country,
  CAST(ar.""Number"" as TEXT) as Asn,
  ar.""Description"" as AsnDescription,
  COUNT(*) as Events,
  COUNT(DISTINCT te.""SourceAddress"") as UniqueIps,
  COUNT(DISTINCT te.""Category"") as Categories
FROM ""ThreatEvents"" te
JOIN ""Countries"" c ON te.""SourceCountryId"" = c.""Id""
JOIN ""AsnRegistries"" ar ON te.""AsnRegistryId"" = ar.""Id""
WHERE te.""DeletedAt"" IS NULL 
  AND te.""Timestamp"" BETWEEN @start AND @end
  AND te.""AsnRegistryId"" IN (SELECT ""AsnRegistryId"" FROM ""TenantAsnRegistries"" WHERE ""TenantId"" = @tenantId AND ""DeletedAt"" IS NULL)
GROUP BY c.""Name"", ar.""Number"", ar.""Description""
ORDER BY Events DESC
LIMIT @limit";

        using var connection = CreateConnection();
        var result = await connection.QueryAsync<CountryAsnCorrelationDto>(sql, new { start, end, tenantId, limit }, commandTimeout: 300);
        return result.AsList();
    }

    public async Task<List<RegionalTimeZoneActivityDto>> GetRegionalTimeZoneActivityAsync(Guid tenantId, DateTime start, DateTime end, int limit = 40, CancellationToken cancellationToken = default)
    {
        var sql = @"
SELECT 
  CASE 
    WHEN EXTRACT(HOUR FROM te.""Timestamp"") BETWEEN 0 AND 5 THEN 'Night Hours (00-05 UTC)'
    WHEN EXTRACT(HOUR FROM te.""Timestamp"") BETWEEN 6 AND 11 THEN 'Morning Hours (06-11 UTC)'
    WHEN EXTRACT(HOUR FROM te.""Timestamp"") BETWEEN 12 AND 17 THEN 'Afternoon Hours (12-17 UTC)'
    WHEN EXTRACT(HOUR FROM te.""Timestamp"") BETWEEN 18 AND 23 THEN 'Evening Hours (18-23 UTC)'
  END as TimePeriod,
  c.""Name"" as Country,
  COUNT(*) as Events
FROM ""ThreatEvents"" te
JOIN ""Countries"" c ON te.""SourceCountryId"" = c.""Id""
WHERE te.""DeletedAt"" IS NULL 
  AND te.""Timestamp"" BETWEEN @start AND @end
  AND te.""AsnRegistryId"" IN (SELECT ""AsnRegistryId"" FROM ""TenantAsnRegistries"" WHERE ""TenantId"" = @tenantId AND ""DeletedAt"" IS NULL)
GROUP BY 
  CASE 
    WHEN EXTRACT(HOUR FROM te.""Timestamp"") BETWEEN 0 AND 5 THEN 'Night Hours (00-05 UTC)'
    WHEN EXTRACT(HOUR FROM te.""Timestamp"") BETWEEN 6 AND 11 THEN 'Morning Hours (06-11 UTC)'
    WHEN EXTRACT(HOUR FROM te.""Timestamp"") BETWEEN 12 AND 17 THEN 'Afternoon Hours (12-17 UTC)'
    WHEN EXTRACT(HOUR FROM te.""Timestamp"") BETWEEN 18 AND 23 THEN 'Evening Hours (18-23 UTC)'
  END, c.""Name""
HAVING COUNT(*) > 10
ORDER BY Events DESC
LIMIT @limit";

        using var connection = CreateConnection();
        var result = await connection.QueryAsync<RegionalTimeZoneActivityDto>(sql, new { start, end, tenantId, limit }, commandTimeout: 300);
        return result.AsList();
    }

    public async Task<List<CrossBorderAttackFlowDto>> GetCrossBorderAttackFlowsAsync(Guid tenantId, DateTime start, DateTime end, int limit = 30, CancellationToken cancellationToken = default)
    {
        var sql = @"
WITH country_category AS (
  SELECT 
    c.""Name"" as SourceCountry,
    dc.""Name"" as DestinationCountry,
    te.""Category"",
    COUNT(*) as Events
  FROM ""ThreatEvents"" te
  JOIN ""Countries"" c ON te.""SourceCountryId"" = c.""Id""
  LEFT JOIN ""Countries"" dc ON te.""DestinationCountryId"" = dc.""Id""
  WHERE te.""DeletedAt"" IS NULL 
    AND te.""Timestamp"" BETWEEN @start AND @end
    AND te.""DestinationCountryId"" IS NOT NULL
    AND te.""AsnRegistryId"" IN (SELECT ""AsnRegistryId"" FROM ""TenantAsnRegistries"" WHERE ""TenantId"" = @tenantId AND ""DeletedAt"" IS NULL)
  GROUP BY c.""Name"", dc.""Name"", te.""Category""
  HAVING COUNT(*) > 10
)
SELECT 
  SourceCountry,
  DestinationCountry, 
  ""Category"",
  Events
FROM country_category
ORDER BY Events DESC
LIMIT @limit";

        using var connection = CreateConnection();
        var result = await connection.QueryAsync<CrossBorderAttackFlowDto>(sql, new { start, end, tenantId, limit }, commandTimeout: 300);
        return result.AsList();
    }

    public async Task<List<CountryCategoryTimelineDto>> GetCountryCategoryTimelineAsync(Guid tenantId, DateTime start, DateTime end, TimeSpan interval, int topCountries = 5, CancellationToken cancellationToken = default)
    {
        var intervalStr = interval.TotalDays >= 1 ? "day" : interval.TotalHours >= 1 ? "hour" : "minute";
        var sql = $@"
SELECT 
  date_trunc('{intervalStr}', te.""Timestamp"") as Time,
  te.""Category"",
  c.""Name"" as Country,
  COUNT(*) as Events
FROM ""ThreatEvents"" te
JOIN ""Countries"" c ON te.""SourceCountryId"" = c.""Id""
WHERE te.""DeletedAt"" IS NULL 
  AND te.""Timestamp"" BETWEEN @start AND @end
  AND te.""AsnRegistryId"" IN (SELECT ""AsnRegistryId"" FROM ""TenantAsnRegistries"" WHERE ""TenantId"" = @tenantId AND ""DeletedAt"" IS NULL)
  AND c.""Name"" IN (
    SELECT c2.""Name"" 
    FROM ""ThreatEvents"" te2
    JOIN ""Countries"" c2 ON te2.""SourceCountryId"" = c2.""Id""
    WHERE te2.""DeletedAt"" IS NULL 
      AND te2.""Timestamp"" BETWEEN @start AND @end
      AND te2.""AsnRegistryId"" IN (SELECT ""AsnRegistryId"" FROM ""TenantAsnRegistries"" WHERE ""TenantId"" = @tenantId AND ""DeletedAt"" IS NULL)
    GROUP BY c2.""Name""
    ORDER BY COUNT(*) DESC
    LIMIT @topCountries
  )
GROUP BY Time, te.""Category"", c.""Name""
ORDER BY Time";

        using var connection = CreateConnection();
        var result = await connection.QueryAsync<CountryCategoryTimelineDto>(sql, new { start, end, tenantId, topCountries }, commandTimeout: 300);
        return result.AsList();
    }
}