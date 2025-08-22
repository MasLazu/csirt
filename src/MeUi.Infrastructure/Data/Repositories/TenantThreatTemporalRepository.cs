using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatTemporal;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace MeUi.Infrastructure.Data.Repositories;

public class TenantThreatTemporalRepository : ITenantThreatTemporalRepository
{
    private readonly string _connectionString;

    public TenantThreatTemporalRepository(IConfiguration configuration)
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

    public async Task<List<TimeSeriesPointDto>> Get24HourAttackPatternAsync(Guid tenantId, DateTime start, DateTime end, CancellationToken cancellationToken = default)
    {
        var sql = @"
SELECT 
  date_trunc('hour', ""Timestamp"") as Time,
  COUNT(*) as Events
FROM ""ThreatEvents"" te
WHERE te.""DeletedAt"" IS NULL 
  AND te.""Timestamp"" BETWEEN @start AND @end
  AND te.""AsnRegistryId"" IN (SELECT ""AsnRegistryId"" FROM ""TenantAsnRegistries"" WHERE ""TenantId"" = @tenantId AND ""DeletedAt"" IS NULL)
GROUP BY date_trunc('hour', ""Timestamp"")
ORDER BY Time";

        using var connection = CreateConnection();
        var result = await connection.QueryAsync<TimeSeriesPointDto>(sql, new { start, end, tenantId }, commandTimeout: 300);
        return result.AsList();
    }

    public async Task<List<DayOfWeekDto>> GetWeeklyAttackDistributionAsync(Guid tenantId, DateTime start, DateTime end, CancellationToken cancellationToken = default)
    {
        var sql = @"
SELECT 
  CASE 
    WHEN EXTRACT(DOW FROM ""Timestamp"") = 0 THEN 'Sunday'
    WHEN EXTRACT(DOW FROM ""Timestamp"") = 1 THEN 'Monday'
    WHEN EXTRACT(DOW FROM ""Timestamp"") = 2 THEN 'Tuesday'
    WHEN EXTRACT(DOW FROM ""Timestamp"") = 3 THEN 'Wednesday'
    WHEN EXTRACT(DOW FROM ""Timestamp"") = 4 THEN 'Thursday'
    WHEN EXTRACT(DOW FROM ""Timestamp"") = 5 THEN 'Friday'
    WHEN EXTRACT(DOW FROM ""Timestamp"") = 6 THEN 'Saturday'
  END as DayOfWeek,
  COUNT(*) as Events
FROM ""ThreatEvents"" te
WHERE te.""DeletedAt"" IS NULL 
  AND te.""Timestamp"" BETWEEN @start AND @end
  AND te.""AsnRegistryId"" IN (SELECT ""AsnRegistryId"" FROM ""TenantAsnRegistries"" WHERE ""TenantId"" = @tenantId AND ""DeletedAt"" IS NULL)
GROUP BY EXTRACT(DOW FROM ""Timestamp"")
ORDER BY EXTRACT(DOW FROM ""Timestamp"")";

        using var connection = CreateConnection();
        var result = await connection.QueryAsync<DayOfWeekDto>(sql, new { start, end, tenantId }, commandTimeout: 300);
        return result.AsList();
    }

    public async Task<List<HourDayHeatmapDto>> GetHourDayHeatmapAsync(Guid tenantId, DateTime start, DateTime end, CancellationToken cancellationToken = default)
    {
        var sql = @"
SELECT 
  EXTRACT(HOUR FROM ""Timestamp"") as Hour,
  CASE 
    WHEN EXTRACT(DOW FROM ""Timestamp"") = 0 THEN 'Sunday'
    WHEN EXTRACT(DOW FROM ""Timestamp"") = 1 THEN 'Monday'
    WHEN EXTRACT(DOW FROM ""Timestamp"") = 2 THEN 'Tuesday'
    WHEN EXTRACT(DOW FROM ""Timestamp"") = 3 THEN 'Wednesday'
    WHEN EXTRACT(DOW FROM ""Timestamp"") = 4 THEN 'Thursday'
    WHEN EXTRACT(DOW FROM ""Timestamp"") = 5 THEN 'Friday'
    WHEN EXTRACT(DOW FROM ""Timestamp"") = 6 THEN 'Saturday'
  END as DayOfWeek,
  COUNT(*) as Events
FROM ""ThreatEvents"" te
WHERE te.""DeletedAt"" IS NULL 
  AND te.""Timestamp"" BETWEEN @start AND @end
  AND te.""AsnRegistryId"" IN (SELECT ""AsnRegistryId"" FROM ""TenantAsnRegistries"" WHERE ""TenantId"" = @tenantId AND ""DeletedAt"" IS NULL)
GROUP BY EXTRACT(HOUR FROM ""Timestamp""), EXTRACT(DOW FROM ""Timestamp"")
ORDER BY Hour, EXTRACT(DOW FROM ""Timestamp"")";

        using var connection = CreateConnection();
        var result = await connection.QueryAsync<HourDayHeatmapDto>(sql, new { start, end, tenantId }, commandTimeout: 300);
        return result.AsList();
    }

    public async Task<List<PeakActivityDto>> GetPeakActivityHoursAsync(Guid tenantId, DateTime start, DateTime end, int limit = 50, CancellationToken cancellationToken = default)
    {
        var sql = @"
SELECT 
  EXTRACT(HOUR FROM ""Timestamp"") as Hour,
  ""Category"" as AttackCategory,
  COUNT(*) as TotalEvents,
  COUNT(DISTINCT ""SourceAddress"") as UniqueSources,
  COUNT(DISTINCT ""DestinationPort"") as PortsTargeted,
  ROUND(COUNT(*)::numeric / 7, 2) as AvgEventsPerHour
FROM ""ThreatEvents"" te
WHERE te.""DeletedAt"" IS NULL 
  AND te.""Timestamp"" BETWEEN @start AND @end
  AND te.""AsnRegistryId"" IN (SELECT ""AsnRegistryId"" FROM ""TenantAsnRegistries"" WHERE ""TenantId"" = @tenantId AND ""DeletedAt"" IS NULL)
GROUP BY EXTRACT(HOUR FROM ""Timestamp""), ""Category""
HAVING COUNT(*) > 5
ORDER BY TotalEvents DESC
LIMIT @limit";

        using var connection = CreateConnection();
        var result = await connection.QueryAsync<PeakActivityDto>(sql, new { start, end, tenantId, limit }, commandTimeout: 300);
        return result.AsList();
    }

    public async Task<List<TimePeriodSeriesDto>> GetTimeOfDayPatternsAsync(Guid tenantId, DateTime start, DateTime end, TimeSpan interval, CancellationToken cancellationToken = default)
    {
        var intervalStr = interval.TotalDays >= 1 ? "day" : interval.TotalHours >= 1 ? "hour" : "minute";
        var sql = $@"
WITH time_patterns AS (
  SELECT 
    date_trunc('{intervalStr}', ""Timestamp"") as time,
    CASE 
      WHEN EXTRACT(HOUR FROM ""Timestamp"") BETWEEN 6 AND 12 THEN 'Morning (6-12)'
      WHEN EXTRACT(HOUR FROM ""Timestamp"") BETWEEN 13 AND 18 THEN 'Afternoon (13-18)'
      WHEN EXTRACT(HOUR FROM ""Timestamp"") BETWEEN 19 AND 23 THEN 'Evening (19-23)'
      ELSE 'Night (0-5)'
    END as TimePeriod,
    COUNT(*) as Events
  FROM ""ThreatEvents"" te
  WHERE te.""DeletedAt"" IS NULL 
    AND te.""Timestamp"" BETWEEN @start AND @end
    AND te.""AsnRegistryId"" IN (SELECT ""AsnRegistryId"" FROM ""TenantAsnRegistries"" WHERE ""TenantId"" = @tenantId AND ""DeletedAt"" IS NULL)
  GROUP BY time, EXTRACT(HOUR FROM ""Timestamp"")
)
SELECT 
  time as Time,
  TimePeriod,
  SUM(Events) as Events
FROM time_patterns
GROUP BY time, TimePeriod
ORDER BY time";

        using var connection = CreateConnection();
        var result = await connection.QueryAsync<TimePeriodSeriesDto>(sql, new { start, end, tenantId }, commandTimeout: 300);
        return result.AsList();
    }

    public async Task<List<TimeSeriesPointDto>> GetWeekdayWeekendTrendsAsync(Guid tenantId, DateTime start, DateTime end, CancellationToken cancellationToken = default)
    {
        var sql = @"
SELECT 
  date_trunc('day', ""Timestamp"") as Time,
  COUNT(*) as Events
FROM ""ThreatEvents"" te
WHERE te.""DeletedAt"" IS NULL 
  AND te.""Timestamp"" BETWEEN @start AND @end
  AND te.""AsnRegistryId"" IN (SELECT ""AsnRegistryId"" FROM ""TenantAsnRegistries"" WHERE ""TenantId"" = @tenantId AND ""DeletedAt"" IS NULL)
GROUP BY date_trunc('day', ""Timestamp""), EXTRACT(DOW FROM ""Timestamp"")
ORDER BY Time";

        using var connection = CreateConnection();
        var result = await connection.QueryAsync<TimeSeriesPointDto>(sql, new { start, end, tenantId }, commandTimeout: 300);
        return result.AsList();
    }

    public async Task<List<CampaignDurationDto>> GetCampaignDurationAnalysisAsync(Guid tenantId, DateTime start, DateTime end, int limit = 30, CancellationToken cancellationToken = default)
    {
        var sql = @"
WITH campaign_duration AS (
  SELECT 
    CAST(te.""SourceAddress"" as TEXT) as SourceIp,
    c.""Name"" as Country,
    MIN(te.""Timestamp"") as first_seen,
    MAX(te.""Timestamp"") as last_seen,
    COUNT(*) as total_events,
    EXTRACT(EPOCH FROM (MAX(te.""Timestamp"") - MIN(te.""Timestamp""))) / 3600 as duration_hours
  FROM ""ThreatEvents"" te
  JOIN ""Countries"" c ON te.""SourceCountryId"" = c.""Id""
  WHERE te.""DeletedAt"" IS NULL 
    AND te.""Timestamp"" BETWEEN @start AND @end
    AND te.""AsnRegistryId"" IN (SELECT ""AsnRegistryId"" FROM ""TenantAsnRegistries"" WHERE ""TenantId"" = @tenantId AND ""DeletedAt"" IS NULL)
  GROUP BY te.""SourceAddress"", c.""Name""
  HAVING COUNT(*) > 20
)
SELECT 
  SourceIp,
  Country,
  total_events as TotalEvents,
  ROUND(duration_hours::numeric, 2) as CampaignDurationHours,
  ROUND((total_events::numeric / GREATEST(duration_hours, 1))::numeric, 2) as EventsPerHour,
  first_seen as CampaignStart,
  last_seen as CampaignEnd
FROM campaign_duration
WHERE duration_hours > 1
ORDER BY EventsPerHour DESC
LIMIT @limit";

        using var connection = CreateConnection();
        var result = await connection.QueryAsync<CampaignDurationDto>(sql, new { start, end, tenantId, limit }, commandTimeout: 300);
        return result.AsList();
    }

    public async Task<List<MonthlyGrowthDto>> GetMonthlyGrowthAnalysisAsync(Guid tenantId, int limit = 40, CancellationToken cancellationToken = default)
    {
        var sql = @"
WITH monthly_trends AS (
  SELECT 
    DATE_TRUNC('month', ""Timestamp"") as month,
    ""Category"",
    COUNT(*) as events
  FROM ""ThreatEvents"" te
  WHERE te.""DeletedAt"" IS NULL 
    AND ""Timestamp"" >= NOW() - INTERVAL '12 months'
    AND te.""AsnRegistryId"" IN (SELECT ""AsnRegistryId"" FROM ""TenantAsnRegistries"" WHERE ""TenantId"" = @tenantId AND ""DeletedAt"" IS NULL)
  GROUP BY DATE_TRUNC('month', ""Timestamp""), ""Category""
),
monthly_growth AS (
  SELECT 
    month,
    ""Category"",
    events,
    LAG(events) OVER (PARTITION BY ""Category"" ORDER BY month) as prev_month_events,
    CASE 
      WHEN LAG(events) OVER (PARTITION BY ""Category"" ORDER BY month) > 0 THEN
        ROUND(((events - LAG(events) OVER (PARTITION BY ""Category"" ORDER BY month))::numeric / 
               LAG(events) OVER (PARTITION BY ""Category"" ORDER BY month) * 100)::numeric, 2)
      ELSE 0
    END as growth_rate
  FROM monthly_trends
)
SELECT 
  TO_CHAR(month, 'YYYY-MM') as Month,
  ""Category"" as AttackCategory,
  events as Events,
  prev_month_events as PreviousMonth,
  growth_rate as GrowthRate
FROM monthly_growth
WHERE prev_month_events IS NOT NULL
  AND events > 50
ORDER BY month DESC, GrowthRate DESC
LIMIT @limit";

        using var connection = CreateConnection();
        var result = await connection.QueryAsync<MonthlyGrowthDto>(sql, new { tenantId, limit }, commandTimeout: 300);
        return result.AsList();
    }
}