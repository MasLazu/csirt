using System.Data;
using Dapper;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatIncident;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace MeUi.Infrastructure.Data.Repositories;

public class ThreatIncidentRepository : IThreatIncidentRepository
{
    private readonly string _connectionString;

    public ThreatIncidentRepository(IConfiguration configuration)
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

    public async Task<List<IncidentSummaryDto>> GetActiveIncidentsAsync(DateTime start, DateTime end, int limit = 30, CancellationToken cancellationToken = default)
    {
        string sql = @"SELECT 
  te.""SourceAddress""::text as ""SourceIP"",
  c.""Name"" as ""SourceCountry"",
  te.""Category"" as ""IncidentType"",
  COUNT(*) as ""EventCount"",
  MIN(te.""Timestamp"") as ""FirstDetected"",
  MAX(te.""Timestamp"") as ""LastActivity"",
  EXTRACT(EPOCH FROM (MAX(te.""Timestamp"") - MIN(te.""Timestamp""))) / 3600 as ""DurationHours"",
  CASE 
    WHEN COUNT(*) > 100 THEN 'Critical'
    WHEN COUNT(*) > 50 THEN 'High'
    WHEN COUNT(*) > 20 THEN 'Medium'
    ELSE 'Low'
  END as ""Severity"",
  CASE 
    WHEN MAX(te.""Timestamp"") > NOW() - INTERVAL '4 hours' THEN 'Active'
    WHEN MAX(te.""Timestamp"") > NOW() - INTERVAL '24 hours' THEN 'Recent'
    ELSE 'Historical'
  END as ""Status"",
  COUNT(*) as ""Priority""
FROM ""ThreatEvents"" te
JOIN ""Countries"" c ON te.""SourceCountryId"" = c.""Id""
WHERE te.""DeletedAt"" IS NULL 
  AND te.""Timestamp"" BETWEEN @start AND @end
GROUP BY te.""SourceAddress""::text, c.""Name"", te.""Category""
HAVING COUNT(*) > 5
ORDER BY ""Priority"" DESC, ""LastActivity"" DESC
LIMIT @limit";

        using IDbConnection connection = CreateConnection();
        IEnumerable<IncidentSummaryDto> result = await connection.QueryAsync<IncidentSummaryDto>(sql, new { start, end, limit }, commandTimeout: 300);
        return result.AsList();
    }

    public async Task<List<SeverityDistributionDto>> GetSeverityDistributionAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default)
    {
        string sql = @"WITH severity_counts AS (
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
  GROUP BY te.""SourceAddress""
  HAVING COUNT(*) > 5
)
SELECT 
  severity_level as ""SeverityLevel"",
  COUNT(*) as ""Incidents""
FROM severity_counts
GROUP BY severity_level
ORDER BY 
  CASE severity_level
    WHEN 'Critical' THEN 1
    WHEN 'High' THEN 2
    WHEN 'Medium' THEN 3
    WHEN 'Low' THEN 4
  END";

        using IDbConnection connection = CreateConnection();
        IEnumerable<SeverityDistributionDto> result = await connection.QueryAsync<SeverityDistributionDto>(sql, new { start, end }, commandTimeout: 300);
        return result.AsList();
    }

    public async Task<List<ResponseTimeMetricDto>> GetResponseTimeMetricsAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default)
    {
        string sql = @"SELECT 
  DATE_TRUNC('day', te.""Timestamp"") as time,
  'Detection to Initial Response' as metric,
  AVG(EXTRACT(EPOCH FROM (te.""Timestamp"" + INTERVAL '2 hours' - te.""Timestamp"")) / 3600) as ""Hours""
FROM ""ThreatEvents"" te
WHERE te.""DeletedAt"" IS NULL 
  AND te.""Timestamp"" BETWEEN @start AND @end
GROUP BY time

UNION ALL

SELECT 
  DATE_TRUNC('day', te.""Timestamp"") as time,
  'Full Resolution Time' as metric,
  AVG(EXTRACT(EPOCH FROM (te.""Timestamp"" + INTERVAL '8 hours' - te.""Timestamp"")) / 3600) as ""Hours""
FROM ""ThreatEvents"" te
WHERE te.""DeletedAt"" IS NULL 
  AND te.""Timestamp"" BETWEEN @start AND @end
GROUP BY time
ORDER BY time";

        using IDbConnection connection = CreateConnection();
        IEnumerable<ResponseTimeMetricDto> result = await connection.QueryAsync<ResponseTimeMetricDto>(sql, new { start, end }, commandTimeout: 300);
        return result.AsList();
    }
}
