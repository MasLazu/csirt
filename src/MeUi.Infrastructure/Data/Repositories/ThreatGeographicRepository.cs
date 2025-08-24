using System.Data;
using Dapper;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatGeographic;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace MeUi.Infrastructure.Data.Repositories;

public class ThreatGeographicRepository : IThreatGeographicRepository
{
    private readonly string _connectionString;

    public ThreatGeographicRepository(IConfiguration configuration)
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

    public async Task<List<CountryAttackTrendPointDto>> GetCountryAttackTrendsAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default)
    {
        string sql = @"SELECT 
  DATE_TRUNC('day', te.""Timestamp"") as time,
  c.""Name"" as ""Country"",
  COUNT(*) as ""Events""
FROM ""ThreatEvents"" te
JOIN ""Countries"" c ON te.""SourceCountryId"" = c.""Id""
WHERE te.""DeletedAt"" IS NULL 
  AND te.""Timestamp"" BETWEEN @start AND @end
GROUP BY time, c.""Name""
HAVING COUNT(*) > 5
ORDER BY time";

        using IDbConnection connection = CreateConnection();
        IEnumerable<CountryAttackTrendPointDto> result = await connection.QueryAsync<CountryAttackTrendPointDto>(sql, new { start, end }, commandTimeout: 300);
        return result.AsList();
    }

    public async Task<List<CountryAttackRankingDto>> GetCountryRankingsAsync(DateTime start, DateTime end, int limit = 20, CancellationToken cancellationToken = default)
    {
        string sql = @"SELECT 
  c.""Name"" as ""Country"",
  COUNT(*) as ""Events""
FROM ""ThreatEvents"" te
JOIN ""Countries"" c ON te.""SourceCountryId"" = c.""Id""
WHERE te.""DeletedAt"" IS NULL 
  AND te.""Timestamp"" BETWEEN @start AND @end
GROUP BY c.""Name""
ORDER BY ""Events"" DESC
LIMIT @limit";

        using IDbConnection connection = CreateConnection();
        IEnumerable<CountryAttackRankingDto> result = await connection.QueryAsync<CountryAttackRankingDto>(sql, new { start, end, limit }, commandTimeout: 300);
        return result.AsList();
    }

    public async Task<List<CountryAsnCorrelationDto>> GetCountryAsnCorrelationAsync(DateTime start, DateTime end, int limit = 25, CancellationToken cancellationToken = default)
    {
        string sql = @"SELECT 
  c.""Name"" as ""Country"",
  ar.""Number"" as ""ASN"",
  ar.""Description"" as ""ASNDescription"",
  COUNT(*) as ""Events"",
  COUNT(DISTINCT ""SourceAddress"") as ""UniqueIPs"",
  COUNT(DISTINCT ""Category"") as ""Categories""
FROM ""ThreatEvents"" te
JOIN ""Countries"" c ON te.""SourceCountryId"" = c.""Id""
JOIN ""AsnRegistries"" ar ON te.""AsnRegistryId"" = ar.""Id""
WHERE te.""DeletedAt"" IS NULL 
  AND te.""Timestamp"" BETWEEN @start AND @end
GROUP BY c.""Name"", ar.""Number"", ar.""Description""
ORDER BY ""Events"" DESC
LIMIT @limit";

        using IDbConnection connection = CreateConnection();
        IEnumerable<CountryAsnCorrelationDto> result = await connection.QueryAsync<CountryAsnCorrelationDto>(sql, new { start, end, limit }, commandTimeout: 300);
        return result.AsList();
    }

    public async Task<List<RegionalTimeBucketDto>> GetRegionalTimeActivityAsync(DateTime start, DateTime end, int limit = 40, CancellationToken cancellationToken = default)
    {
        string sql = @"SELECT 
  CASE 
    WHEN EXTRACT(HOUR FROM ""Timestamp"") BETWEEN 0 AND 5 THEN 'Night Hours (00-05 UTC)'
    WHEN EXTRACT(HOUR FROM ""Timestamp"") BETWEEN 6 AND 11 THEN 'Morning Hours (06-11 UTC)'
    WHEN EXTRACT(HOUR FROM ""Timestamp"") BETWEEN 12 AND 17 THEN 'Afternoon Hours (12-17 UTC)'
    WHEN EXTRACT(HOUR FROM ""Timestamp"") BETWEEN 18 AND 23 THEN 'Evening Hours (18-23 UTC)'
  END as ""TimePeriod"",
  c.""Name"" as ""Country"",
  COUNT(*) as ""Events""
FROM ""ThreatEvents"" te
JOIN ""Countries"" c ON te.""SourceCountryId"" = c.""Id""
WHERE te.""DeletedAt"" IS NULL 
  AND te.""Timestamp"" BETWEEN @start AND @end
GROUP BY 
  CASE 
    WHEN EXTRACT(HOUR FROM ""Timestamp"") BETWEEN 0 AND 5 THEN 'Night Hours (00-05 UTC)'
    WHEN EXTRACT(HOUR FROM ""Timestamp"") BETWEEN 6 AND 11 THEN 'Morning Hours (06-11 UTC)'
    WHEN EXTRACT(HOUR FROM ""Timestamp"") BETWEEN 12 AND 17 THEN 'Afternoon Hours (12-17 UTC)'
    WHEN EXTRACT(HOUR FROM ""Timestamp"") BETWEEN 18 AND 23 THEN 'Evening Hours (18-23 UTC)'
  END, c.""Name""
HAVING COUNT(*) > 10
ORDER BY ""Events"" DESC
LIMIT @limit";

        using IDbConnection connection = CreateConnection();
        IEnumerable<RegionalTimeBucketDto> result = await connection.QueryAsync<RegionalTimeBucketDto>(sql, new { start, end, limit }, commandTimeout: 300);
        return result.AsList();
    }

    public async Task<List<CrossBorderFlowDto>> GetCrossBorderFlowsAsync(DateTime start, DateTime end, int limit = 30, CancellationToken cancellationToken = default)
    {
        string sql = @"WITH country_category AS (
  SELECT 
    c.""Name"" as ""SourceCountry"",
    dc.""Name"" as ""DestinationCountry"",
    ""Category"",
    COUNT(*) as ""Events""
  FROM ""ThreatEvents"" te
  JOIN ""Countries"" c ON te.""SourceCountryId"" = c.""Id""
  LEFT JOIN ""Countries"" dc ON te.""DestinationCountryId"" = dc.""Id""
  WHERE te.""DeletedAt"" IS NULL 
    AND te.""Timestamp"" BETWEEN @start AND @end
    AND te.""DestinationCountryId"" IS NOT NULL
  GROUP BY c.""Name"", dc.""Name"", ""Category""
  HAVING COUNT(*) > 10
)
SELECT 
  ""SourceCountry"",
  ""DestinationCountry"", 
  ""Category"",
  ""Events""
FROM country_category
ORDER BY ""Events"" DESC
LIMIT @limit";

        using IDbConnection connection = CreateConnection();
        IEnumerable<CrossBorderFlowDto> result = await connection.QueryAsync<CrossBorderFlowDto>(sql, new { start, end, limit }, commandTimeout: 300);
        return result.AsList();
    }

    public async Task<List<CategoryCountryTrendPointDto>> GetCategoryCountryTrendAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default)
    {
        string sql = @"SELECT 
  DATE_TRUNC('day', te.""Timestamp"") as time,
  ""Category"",
  c.""Name"" as ""Country"",
  COUNT(*) as ""Events""
FROM ""ThreatEvents"" te
JOIN ""Countries"" c ON te.""SourceCountryId"" = c.""Id""
WHERE te.""DeletedAt"" IS NULL 
  AND te.""Timestamp"" BETWEEN @start AND @end
  AND c.""Name"" IN (
    SELECT c2.""Name"" 
    FROM ""ThreatEvents"" te2
    JOIN ""Countries"" c2 ON te2.""SourceCountryId"" = c2.""Id""
    WHERE te2.""DeletedAt"" IS NULL 
      AND te2.""Timestamp"" BETWEEN @start AND @end
    GROUP BY c2.""Name""
    ORDER BY COUNT(*) DESC
    LIMIT 5
  )
GROUP BY time, ""Category"", c.""Name""
ORDER BY time";

        using IDbConnection connection = CreateConnection();
        IEnumerable<CategoryCountryTrendPointDto> result = await connection.QueryAsync<CategoryCountryTrendPointDto>(sql, new { start, end }, commandTimeout: 300);
        return result.AsList();
    }
}
