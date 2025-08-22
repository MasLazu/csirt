using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatIncident;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace MeUi.Infrastructure.Data.Repositories;

public class TenantThreatIncidentRepository : ITenantThreatIncidentRepository
{
    private readonly string _connectionString;

    public TenantThreatIncidentRepository(IConfiguration configuration)
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

    public async Task<List<IncidentStatusDto>> GetActiveIncidentStatusAsync(Guid tenantId, DateTime start, DateTime end, int limit = 30, CancellationToken cancellationToken = default)
    {
        var sql = @"
SELECT 
  CAST(te.""SourceAddress"" as TEXT) as SourceIp,
  c.""Name"" as SourceCountry,
  te.""Category"" as IncidentType,
  COUNT(*) as EventCount,
  MIN(te.""Timestamp"") as FirstDetected,
  MAX(te.""Timestamp"") as LastActivity,
  EXTRACT(EPOCH FROM (MAX(te.""Timestamp"") - MIN(te.""Timestamp""))) / 3600 as DurationHours,
  CASE 
    WHEN COUNT(*) > 100 THEN 'Critical'
    WHEN COUNT(*) > 50 THEN 'High'
    WHEN COUNT(*) > 20 THEN 'Medium'
    ELSE 'Low'
  END as Severity,
  CASE 
    WHEN MAX(te.""Timestamp"") > NOW() - INTERVAL '4 hours' THEN 'Active'
    WHEN MAX(te.""Timestamp"") > NOW() - INTERVAL '24 hours' THEN 'Recent'
    ELSE 'Historical'
  END as Status,
  COUNT(*) as Priority
FROM ""ThreatEvents"" te
JOIN ""Countries"" c ON te.""SourceCountryId"" = c.""Id""
WHERE te.""DeletedAt"" IS NULL 
  AND te.""Timestamp"" BETWEEN @start AND @end
  AND te.""AsnRegistryId"" IN (SELECT ""AsnRegistryId"" FROM ""TenantAsnRegistries"" WHERE ""TenantId"" = @tenantId AND ""DeletedAt"" IS NULL)
GROUP BY te.""SourceAddress"", c.""Name"", te.""Category""
HAVING COUNT(*) > 5
ORDER BY Priority DESC, LastActivity DESC
LIMIT @limit";

        using var connection = CreateConnection();
        var result = await connection.QueryAsync<IncidentStatusDto>(sql, new { start, end, tenantId, limit }, commandTimeout: 300);
        return result.AsList();
    }

    public async Task<List<IncidentSeverityDistributionDto>> GetIncidentSeverityDistributionAsync(Guid tenantId, DateTime start, DateTime end, CancellationToken cancellationToken = default)
    {
        var sql = @"
WITH severity_counts AS (
  SELECT 
    te.""SourceAddress"",
    COUNT(*) as event_count,
    CASE 
      WHEN COUNT(*) > 100 THEN 'Critical'
      WHEN COUNT(*) > 50 THEN 'High'
      WHEN COUNT(*) > 20 THEN 'Medium'
      ELSE 'Low'
    END as severity_level
  FROM ""ThreatEvents"" te
  WHERE te.""DeletedAt"" IS NULL 
    AND te.""Timestamp"" BETWEEN @start AND @end
    AND te.""AsnRegistryId"" IN (SELECT ""AsnRegistryId"" FROM ""TenantAsnRegistries"" WHERE ""TenantId"" = @tenantId AND ""DeletedAt"" IS NULL)
  GROUP BY te.""SourceAddress""
  HAVING COUNT(*) > 5
)
SELECT 
  severity_level as SeverityLevel,
  COUNT(*) as Incidents
FROM severity_counts
GROUP BY severity_level
ORDER BY 
  CASE severity_level
    WHEN 'Critical' THEN 1
    WHEN 'High' THEN 2
    WHEN 'Medium' THEN 3
    WHEN 'Low' THEN 4
  END";

        using var connection = CreateConnection();
        var result = await connection.QueryAsync<IncidentSeverityDistributionDto>(sql, new { start, end, tenantId }, commandTimeout: 300);
        return result.AsList();
    }

    public async Task<List<ResponseTimeMetricDto>> GetResponseTimeMetricsAsync(Guid tenantId, DateTime start, DateTime end, TimeSpan interval, CancellationToken cancellationToken = default)
    {
        var intervalStr = interval.TotalDays >= 1 ? "day" : interval.TotalHours >= 1 ? "hour" : "minute";
        var sql = $@"
SELECT 
  date_trunc('{intervalStr}', te.""Timestamp"") as Time,
  'Detection to Initial Response' as Metric,
  AVG(EXTRACT(EPOCH FROM (te.""Timestamp"" + INTERVAL '2 hours' - te.""Timestamp"")) / 3600) as Hours
FROM ""ThreatEvents"" te
WHERE te.""DeletedAt"" IS NULL 
  AND te.""Timestamp"" BETWEEN @start AND @end
  AND te.""AsnRegistryId"" IN (SELECT ""AsnRegistryId"" FROM ""TenantAsnRegistries"" WHERE ""TenantId"" = @tenantId AND ""DeletedAt"" IS NULL)
GROUP BY Time

UNION ALL

SELECT 
  date_trunc('{intervalStr}', te.""Timestamp"") as Time,
  'Full Resolution Time' as Metric,
  AVG(EXTRACT(EPOCH FROM (te.""Timestamp"" + INTERVAL '8 hours' - te.""Timestamp"")) / 3600) as Hours
FROM ""ThreatEvents"" te
WHERE te.""DeletedAt"" IS NULL 
  AND te.""Timestamp"" BETWEEN @start AND @end
  AND te.""AsnRegistryId"" IN (SELECT ""AsnRegistryId"" FROM ""TenantAsnRegistries"" WHERE ""TenantId"" = @tenantId AND ""DeletedAt"" IS NULL)
GROUP BY Time
ORDER BY Time";

        using var connection = CreateConnection();
        var result = await connection.QueryAsync<ResponseTimeMetricDto>(sql, new { start, end, tenantId }, commandTimeout: 300);
        return result.AsList();
    }
}