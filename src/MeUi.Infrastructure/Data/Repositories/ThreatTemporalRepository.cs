using System.Data;
using Dapper;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatTemporal;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace MeUi.Infrastructure.Data.Repositories;

public class ThreatTemporalRepository : IThreatTemporalRepository
{
    private readonly string _connectionString;

    public ThreatTemporalRepository(IConfiguration configuration)
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

    public async Task<List<TimeSeriesPointDto>> Get24HourAttackPatternAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default)
    {
        var sql = @"SELECT
  DATE_TRUNC('hour', ""Timestamp"") as ""Time"",
  COUNT(*) as ""Events""
FROM ""ThreatEvents""
WHERE ""DeletedAt"" IS NULL
  AND ""Timestamp"" BETWEEN @start AND @end
GROUP BY ""Time""
ORDER BY ""Time""";

        using var connection = CreateConnection();
        var result = await connection.QueryAsync<TimeSeriesPointDto>(sql, new { start, end }, commandTimeout: 300);
        return result.AsList();
    }

    public async Task<List<DayOfWeekDto>> GetWeeklyAttackDistributionAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default)
    {
        var sql = @"SELECT
      TO_CHAR(""Timestamp"", 'FMDay') as ""DayOfWeek"",
  COUNT(*) as ""Events""
FROM ""ThreatEvents""
WHERE ""DeletedAt"" IS NULL
  AND ""Timestamp"" BETWEEN @start AND @end
     GROUP BY TO_CHAR(""Timestamp"", 'FMDay')
ORDER BY MIN(""Timestamp"")";

        using var connection = CreateConnection();
        var result = await connection.QueryAsync<DayOfWeekDto>(sql, new { start, end }, commandTimeout: 300);
        return result.AsList();
    }

    public async Task<List<HourDayHeatmapDto>> GetHourDayHeatmapAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default)
    {
        var sql = @"SELECT
  EXTRACT(HOUR FROM ""Timestamp"")::int as ""Hour"",
  TO_CHAR(""Timestamp"", 'FMDay') as ""DayOfWeek"",
  COUNT(*) as ""Events""
FROM ""ThreatEvents""
WHERE ""DeletedAt"" IS NULL
    AND ""Timestamp"" BETWEEN @start AND @end
GROUP BY 1, 2
ORDER BY 1, 2";

        using var connection = CreateConnection();
        var result = await connection.QueryAsync<HourDayHeatmapDto>(sql, new { start, end }, commandTimeout: 300);
        return result.AsList();
    }

    public async Task<List<PeakActivityDto>> GetPeakActivityByCategoryAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default)
    {
        var sql = @"SELECT
  EXTRACT(HOUR FROM ""Timestamp"")::int as ""Hour"",
  ""Category"" as ""AttackCategory"",
  COUNT(*) as ""TotalEvents"",
  COUNT(DISTINCT ""SourceAddress""::text) as ""UniqueSources"",
  COUNT(DISTINCT ""DestinationPort"") as ""PortsTargeted"",
  AVG(COUNT(*)) OVER (PARTITION BY EXTRACT(HOUR FROM ""Timestamp"")) as ""AvgEventsPerHour""
FROM ""ThreatEvents""
WHERE ""DeletedAt"" IS NULL
  AND ""Timestamp"" BETWEEN @start AND @end
GROUP BY EXTRACT(HOUR FROM ""Timestamp""), ""Category""
ORDER BY ""TotalEvents"" DESC";

        using var connection = CreateConnection();
        var result = await connection.QueryAsync<PeakActivityDto>(sql, new { start, end }, commandTimeout: 300);
        return result.AsList();
    }

    public async Task<List<TimePeriodSeriesDto>> GetAttackPatternsByTimeOfDayAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default)
    {
        var sql = @"SELECT
  DATE_TRUNC('hour', ""Timestamp"") as ""Time"",
  CASE
    WHEN EXTRACT(HOUR FROM ""Timestamp"") BETWEEN 0 AND 5 THEN 'Late Night'
    WHEN EXTRACT(HOUR FROM ""Timestamp"") BETWEEN 6 AND 11 THEN 'Morning'
    WHEN EXTRACT(HOUR FROM ""Timestamp"") BETWEEN 12 AND 17 THEN 'Afternoon'
    ELSE 'Evening'
  END as ""TimePeriod"",
  COUNT(*) as ""Events""
FROM ""ThreatEvents""
WHERE ""DeletedAt"" IS NULL
  AND ""Timestamp"" BETWEEN @start AND @end
GROUP BY ""TimePeriod"", DATE_TRUNC('hour', ""Timestamp"")
ORDER BY ""Time""";

        using var connection = CreateConnection();
        var result = await connection.QueryAsync<TimePeriodSeriesDto>(sql, new { start, end }, commandTimeout: 300);
        return result.AsList();
    }

    public async Task<List<TimeSeriesPointDto>> GetWeekdayWeekendTrendsAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default)
    {
        var sql = @"SELECT
  DATE_TRUNC('day', ""Timestamp"") as ""Time"",
  CASE WHEN EXTRACT(DOW FROM ""Timestamp"") IN (0,6) THEN 'Weekend' ELSE 'Weekday' END as ""TimePeriod"",
  COUNT(*) as ""Events""
FROM ""ThreatEvents""
WHERE ""DeletedAt"" IS NULL
  AND ""Timestamp"" BETWEEN @start AND @end
GROUP BY DATE_TRUNC('day', ""Timestamp""), CASE WHEN EXTRACT(DOW FROM ""Timestamp"") IN (0,6) THEN 'Weekend' ELSE 'Weekday' END
ORDER BY ""Time""";

        using var connection = CreateConnection();
        var result = await connection.QueryAsync<TimeSeriesPointDto>(sql, new { start, end }, commandTimeout: 300);
        return result.AsList();
    }

    public async Task<List<CampaignDurationDto>> GetAttackCampaignDurationAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default)
    {
        var sql = @"SELECT
  te.""SourceAddress""::text as ""SourceIp"",
             coalesce(c.""Name"", '') as ""Country"",
  COUNT(*) as ""TotalEvents"",
  EXTRACT(EPOCH FROM (MAX(te.""Timestamp"") - MIN(te.""Timestamp""))) / 3600.0 as ""CampaignDurationHours"",
  (COUNT(*)::decimal / NULLIF(EXTRACT(EPOCH FROM (MAX(te.""Timestamp"") - MIN(te.""Timestamp""))) / 3600.0, 0)) as ""EventsPerHour"",
  MIN(te.""Timestamp"") as ""CampaignStart"",
  MAX(te.""Timestamp"") as ""CampaignEnd""
FROM ""ThreatEvents"" te
LEFT JOIN ""Countries"" c ON te.""SourceCountryId"" = c.""Id""
WHERE te.""DeletedAt"" IS NULL
  AND te.""Timestamp"" BETWEEN @start AND @end
GROUP BY te.""SourceAddress""::text, c.""Name""
ORDER BY ""TotalEvents"" DESC
LIMIT 50";

        using var connection = CreateConnection();
        var result = await connection.QueryAsync<CampaignDurationDto>(sql, new { start, end }, commandTimeout: 300);
        return result.AsList();
    }

    public async Task<List<MonthlyGrowthDto>> GetMonthlyGrowthRateAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default)
    {
        var sql = @"WITH month_data AS (
  SELECT DATE_TRUNC('month', ""Timestamp"") as ""MonthStart"",
    ""Category"" as ""AttackCategory"",
    COUNT(*) as ""Events""
  FROM ""ThreatEvents""
  WHERE ""DeletedAt"" IS NULL AND ""Timestamp"" BETWEEN @start AND @end
  GROUP BY DATE_TRUNC('month', ""Timestamp""), ""Category""
)
SELECT to_char(m.""MonthStart"", 'YYYY-MM') as ""Month"",
  m.""AttackCategory"" as ""AttackCategory"",
  m.""Events"",
  COALESCE(p.""Events"", 0) as ""PreviousMonth"",
  CASE
    WHEN COALESCE(p.""Events"", 0) = 0 AND m.""Events"" > 0 THEN 2147483647
    WHEN COALESCE(p.""Events"", 0) = 0 THEN 0
    ELSE ROUND(((m.""Events""::decimal - p.""Events""::decimal) / NULLIF(p.""Events""::decimal,0)) * 100, 2)
  END as ""GrowthRate""
FROM month_data m
LEFT JOIN month_data p ON p.""MonthStart"" = m.""MonthStart"" - INTERVAL '1 month' AND p.""AttackCategory"" = m.""AttackCategory""
ORDER BY m.""MonthStart"" DESC";

        using var connection = CreateConnection();
        var result = await connection.QueryAsync<MonthlyGrowthDto>(sql, new { start, end }, commandTimeout: 300);
        return result.AsList();
    }
}
