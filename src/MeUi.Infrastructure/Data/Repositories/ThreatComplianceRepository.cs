using System.Data;
using Dapper;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatCompliance;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace MeUi.Infrastructure.Data.Repositories
{
    public class ThreatComplianceRepository : IThreatComplianceRepository
    {
        private readonly string _connectionString;

        public ThreatComplianceRepository(IConfiguration configuration)
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

        public async Task<List<ExecutiveSummaryDto>> GetExecutiveSummaryAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default)
        {
            var sql = @"WITH monthly_summary AS (
  SELECT 
    DATE_TRUNC('month', te.""Timestamp"") as Month,
    COUNT(*) as TotalThreats,
    COUNT(DISTINCT te.""SourceAddress"") as UniqueSources,
    COUNT(DISTINCT te.""SourceCountryId"") as CountriesAffected,
    COUNT(DISTINCT te.""Category"") as AttackCategories,
    COUNT(DISTINCT te.""DestinationPort"") as PortsTargeted,
    AVG(CASE WHEN te.""Category"" LIKE '%critical%' OR te.""Category"" LIKE '%high%' THEN 10
             WHEN te.""Category"" LIKE '%medium%' THEN 5
             ELSE 1 END) as AvgSeverityScore,
    LAG(COUNT(*)) OVER (ORDER BY DATE_TRUNC('month', te.""Timestamp"")) as PreviousMonth
  FROM ""ThreatEvents"" te
  WHERE te.""DeletedAt"" IS NULL 
    AND te.""Timestamp"" BETWEEN @start AND @end
  GROUP BY DATE_TRUNC('month', te.""Timestamp""))
SELECT 
  TO_CHAR(Month, 'YYYY-MM') as Period,
  TotalThreats as TotalThreats,
  UniqueSources as UniqueSources,
  CountriesAffected as CountriesAffected,
  AttackCategories as AttackCategories,
  PortsTargeted as PortsTargeted,
  ROUND(AvgSeverityScore::numeric, 2) as AvgSeverity,
  CASE 
    WHEN PreviousMonth > 0 THEN 
      ROUND(((TotalThreats - PreviousMonth)::numeric / PreviousMonth * 100)::numeric, 2)
    ELSE 0
  END as GrowthRate,
  CASE 
    WHEN TotalThreats > 10000 THEN 'High'
    WHEN TotalThreats > 5000 THEN 'Medium'
    ELSE 'Low'
  END as ThreatLevel
FROM monthly_summary
WHERE Month >= @start
ORDER BY Month DESC
LIMIT 12";

            using var connection = CreateConnection();
            var result = await connection.QueryAsync<ExecutiveSummaryDto>(sql, new { start, end }, commandTimeout: 300);
            return result.AsList();
        }

        public async Task<List<KpiTrendPointDto>> GetKpiTrendAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default)
        {
            var sql = @"SELECT 
  DATE_TRUNC('day', te.""Timestamp"") as time,
  'Total Threats' as metric,
  COUNT(*) as value
FROM ""ThreatEvents"" te
WHERE te.""DeletedAt"" IS NULL 
  AND te.""Timestamp"" BETWEEN @start AND @end
GROUP BY time

UNION ALL

SELECT 
  DATE_TRUNC('day', te.""Timestamp"") as time,
  'Critical Threats' as metric,
  COUNT(*) as value
FROM ""ThreatEvents"" te
WHERE te.""DeletedAt"" IS NULL 
  AND te.""Timestamp"" BETWEEN @start AND @end
  AND (te.""Category"" LIKE '%critical%' OR te.""Category"" LIKE '%malware%')
GROUP BY time

UNION ALL

SELECT 
  DATE_TRUNC('day', te.""Timestamp"") as time,
  'Unique Sources' as metric,
  COUNT(DISTINCT te.""SourceAddress"") as value
FROM ""ThreatEvents"" te
WHERE te.""DeletedAt"" IS NULL 
  AND te.""Timestamp"" BETWEEN @start AND @end
GROUP BY time
ORDER BY time";

            using var connection = CreateConnection();
            var result = await connection.QueryAsync<KpiTrendPointDto>(sql, new { start, end }, commandTimeout: 300);
            return result.AsList();
        }

        public async Task<ComplianceScoreDto> GetComplianceScoreAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default)
        {
            var sql = @"WITH compliance_metrics AS (
  SELECT 
    COUNT(*) as total_events,
    COUNT(CASE WHEN te.""Timestamp"" > NOW() - INTERVAL '24 hours' THEN 1 END) as recent_events,
    COUNT(DISTINCT te.""SourceCountryId"") as countries_covered,
    COUNT(DISTINCT te.""Category"") as categories_monitored
  FROM ""ThreatEvents"" te
  WHERE te.""DeletedAt"" IS NULL 
    AND te.""Timestamp"" BETWEEN @start AND @end
)
SELECT 
  CASE 
    WHEN countries_covered >= 50 AND categories_monitored >= 10 AND recent_events > 100 THEN 95
    WHEN countries_covered >= 30 AND categories_monitored >= 8 AND recent_events > 50 THEN 85
    WHEN countries_covered >= 20 AND categories_monitored >= 5 AND recent_events > 20 THEN 75
    ELSE 65
  END as ComplianceScore
FROM compliance_metrics";

            using var connection = CreateConnection();
            var result = await connection.QueryFirstOrDefaultAsync<ComplianceScoreDto>(sql, new { start, end }, commandTimeout: 300);
            return result ?? new ComplianceScoreDto { ComplianceScore = 0 };
        }

        public async Task<List<RegionalRiskDto>> GetRegionalRiskAsync(DateTime start, DateTime end, int limit = 15, CancellationToken cancellationToken = default)
        {
            var sql = @"SELECT 
  c.""Name"" as TopRiskCountry,
  COUNT(*) as ThreatEvents,
  COUNT(DISTINCT te.""SourceAddress"") as UniqueSources,
  COUNT(DISTINCT te.""Category"") as AttackTypes,
  ROUND((COUNT(*) * COUNT(DISTINCT te.""SourceAddress""))::numeric / 1000, 2) as RiskScore
FROM ""ThreatEvents"" te
JOIN ""Countries"" c ON te.""SourceCountryId"" = c.""Id""
WHERE te.""DeletedAt"" IS NULL 
  AND te.""Timestamp"" BETWEEN @start AND @end
GROUP BY c.""Name""
ORDER BY RiskScore DESC
LIMIT @limit";

            using var connection = CreateConnection();
            var result = await connection.QueryAsync<RegionalRiskDto>(sql, new { start, end, limit }, commandTimeout: 300);
            return result.AsList();
        }

        public async Task<List<AttackCategoryDto>> GetAttackCategoryAnalysisAsync(DateTime start, DateTime end, int limit = 15, CancellationToken cancellationToken = default)
        {
            var sql = @"SELECT 
  te.""Category"" as AttackCategory,
  COUNT(*) as TotalEvents,
  COUNT(DISTINCT te.""SourceAddress"") as UniqueSources,
  COUNT(DISTINCT te.""SourceCountryId"") as Countries,
  ROUND(AVG(EXTRACT(EPOCH FROM (NOW() - te.""Timestamp"")) / 3600)::numeric, 2) as AvgAgeHours,
  CASE 
    WHEN COUNT(*) > 1000 THEN 'Critical Focus'
    WHEN COUNT(*) > 500 THEN 'High Priority'
    WHEN COUNT(*) > 100 THEN 'Monitor'
    ELSE 'Low Priority'
  END as RecommendedAction
FROM ""ThreatEvents"" te
WHERE te.""DeletedAt"" IS NULL 
  AND te.""Timestamp"" BETWEEN @start AND @end
GROUP BY te.""Category""
ORDER BY TotalEvents DESC
LIMIT @limit";

            using var connection = CreateConnection();
            var result = await connection.QueryAsync<AttackCategoryDto>(sql, new { start, end, limit }, commandTimeout: 300);
            return result.AsList();
        }

        public async Task<RiskLevelDto> GetCurrentRiskLevelAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default)
        {
            var sql = @"WITH risk_calculation AS (
  SELECT 
    COUNT(*) as total_threats,
    COUNT(DISTINCT te.""SourceAddress"") as unique_sources,
    COUNT(CASE WHEN te.""Category"" LIKE '%critical%' OR te.""Category"" LIKE '%malware%' THEN 1 END) as critical_threats
  FROM ""ThreatEvents"" te
  WHERE te.""DeletedAt"" IS NULL 
    AND te.""Timestamp"" BETWEEN @start AND @end
)
SELECT 
  CASE 
    WHEN critical_threats > 100 OR unique_sources > 500 THEN 9
    WHEN critical_threats > 50 OR unique_sources > 250 THEN 7
    WHEN critical_threats > 20 OR unique_sources > 100 THEN 5
    WHEN critical_threats > 10 OR unique_sources > 50 THEN 3
    ELSE 1
  END as RiskLevel
FROM risk_calculation";

            using var connection = CreateConnection();
            var result = await connection.QueryFirstOrDefaultAsync<RiskLevelDto>(sql, new { start, end }, commandTimeout: 300);
            return result ?? new RiskLevelDto { RiskLevel = 1 };
        }
    }
}
