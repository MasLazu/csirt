using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatCompliance;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace MeUi.Infrastructure.Data.Repositories;

public class TenantThreatComplianceRepository : ITenantThreatComplianceRepository
{
    private readonly string _connectionString;

    public TenantThreatComplianceRepository(IConfiguration configuration)
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

    public async Task<List<ExecutiveSummaryDto>> GetExecutiveSummaryAsync(Guid tenantId, DateTime start, DateTime end, CancellationToken cancellationToken = default)
    {
        string sql = @"
WITH monthly_summary AS (
  SELECT 
    DATE_TRUNC('month', te.""Timestamp"") as ""Month"",
    COUNT(*) as ""TotalThreats"",
    COUNT(DISTINCT te.""SourceAddress"") as ""UniqueSources"",
    COUNT(DISTINCT te.""SourceCountryId"") as ""CountriesAffected"",
    COUNT(DISTINCT te.""Category"") as ""AttackCategories"",
    COUNT(DISTINCT te.""DestinationPort"") as ""PortsTargeted"",
    AVG(CASE WHEN te.""Category"" LIKE '%critical%' OR te.""Category"" LIKE '%high%' THEN 10
             WHEN te.""Category"" LIKE '%medium%' THEN 5
             ELSE 1 END) as ""AvgSeverityScore"",
    LAG(COUNT(*)) OVER (ORDER BY DATE_TRUNC('month', te.""Timestamp"")) as ""PreviousMonth""
  FROM ""ThreatEvents"" te
  WHERE te.""DeletedAt"" IS NULL 
    AND te.""Timestamp"" >= @start - INTERVAL '12 months'
    AND te.""AsnRegistryId"" IN (SELECT ""AsnRegistryId"" FROM ""TenantAsnRegistries"" WHERE ""TenantId"" = @tenantId AND ""DeletedAt"" IS NULL)
  GROUP BY DATE_TRUNC('month', te.""Timestamp"")
)
SELECT 
  TO_CHAR(""Month"", 'YYYY-MM') as Period,
  ""TotalThreats"" as TotalThreats,
  ""UniqueSources"" as UniqueSources,
  ""CountriesAffected"" as CountriesAffected,
  ""AttackCategories"" as AttackCategories,
  ""PortsTargeted"" as PortsTargeted,
  ROUND(""AvgSeverityScore""::numeric, 2) as AvgSeverity,
  CASE 
    WHEN ""PreviousMonth"" > 0 THEN 
      ROUND(((""TotalThreats"" - ""PreviousMonth"")::numeric / ""PreviousMonth"" * 100)::numeric, 2)
    ELSE 0
  END as GrowthRate,
  CASE 
    WHEN ""TotalThreats"" > 10000 THEN 'High'
    WHEN ""TotalThreats"" > 5000 THEN 'Medium'
    ELSE 'Low'
  END as ThreatLevel
FROM monthly_summary
WHERE ""Month"" >= @start
ORDER BY ""Month"" DESC
LIMIT 12
";
        using IDbConnection connection = CreateConnection();
        IEnumerable<ExecutiveSummaryDto> result = await connection.QueryAsync<ExecutiveSummaryDto>(sql, new { start, end, tenantId }, commandTimeout: 300);
        return result.AsList();
    }

    public async Task<List<KpiTrendPointDto>> GetKpiTrendAsync(Guid tenantId, DateTime start, DateTime end, CancellationToken cancellationToken = default)
    {
        string sql = @"
SELECT 
  DATE_TRUNC('day', te.""Timestamp"") as Time,
  'Total Threats' as Metric,
  COUNT(*) as Value
FROM ""ThreatEvents"" te
WHERE te.""DeletedAt"" IS NULL 
  AND te.""Timestamp"" BETWEEN @start AND @end
  AND te.""AsnRegistryId"" IN (SELECT ""AsnRegistryId"" FROM ""TenantAsnRegistries"" WHERE ""TenantId"" = @tenantId AND ""DeletedAt"" IS NULL)
GROUP BY Time

UNION ALL

SELECT 
  DATE_TRUNC('day', te.""Timestamp"") as Time,
  'Critical Threats' as Metric,
  COUNT(*) as Value
FROM ""ThreatEvents"" te
WHERE te.""DeletedAt"" IS NULL 
  AND te.""Timestamp"" BETWEEN @start AND @end
  AND (te.""Category"" LIKE '%critical%' OR te.""Category"" LIKE '%malware%')
  AND te.""AsnRegistryId"" IN (SELECT ""AsnRegistryId"" FROM ""TenantAsnRegistries"" WHERE ""TenantId"" = @tenantId AND ""DeletedAt"" IS NULL)
GROUP BY Time

UNION ALL

SELECT 
  DATE_TRUNC('day', te.""Timestamp"") as Time,
  'Unique Sources' as Metric,
  COUNT(DISTINCT te.""SourceAddress"") as Value
FROM ""ThreatEvents"" te
WHERE te.""DeletedAt"" IS NULL 
  AND te.""Timestamp"" BETWEEN @start AND @end
  AND te.""AsnRegistryId"" IN (SELECT ""AsnRegistryId"" FROM ""TenantAsnRegistries"" WHERE ""TenantId"" = @tenantId AND ""DeletedAt"" IS NULL)
GROUP BY Time
ORDER BY Time
";
        using IDbConnection connection = CreateConnection();
        IEnumerable<KpiTrendPointDto> result = await connection.QueryAsync<KpiTrendPointDto>(sql, new { start, end, tenantId }, commandTimeout: 300);
        return result.AsList();
    }

    public async Task<ComplianceScoreDto> GetComplianceScoreAsync(Guid tenantId, DateTime start, DateTime end, CancellationToken cancellationToken = default)
    {
        string sql = @"
WITH compliance_metrics AS (
  SELECT 
    COUNT(*) as total_events,
    COUNT(CASE WHEN te.""Timestamp"" > NOW() - INTERVAL '24 hours' THEN 1 END) as recent_events,
    COUNT(DISTINCT te.""SourceCountryId"") as countries_covered,
    COUNT(DISTINCT te.""Category"") as categories_monitored
  FROM ""ThreatEvents"" te
  WHERE te.""DeletedAt"" IS NULL 
    AND te.""Timestamp"" BETWEEN @start AND @end
    AND te.""AsnRegistryId"" IN (SELECT ""AsnRegistryId"" FROM ""TenantAsnRegistries"" WHERE ""TenantId"" = @tenantId AND ""DeletedAt"" IS NULL)
)
SELECT 
  CASE 
    WHEN countries_covered >= 50 AND categories_monitored >= 10 AND recent_events > 100 THEN 95
    WHEN countries_covered >= 30 AND categories_monitored >= 8 AND recent_events > 50 THEN 85
    WHEN countries_covered >= 20 AND categories_monitored >= 5 AND recent_events > 20 THEN 75
    ELSE 65
  END as ComplianceScore
FROM compliance_metrics
";
        using IDbConnection connection = CreateConnection();
        ComplianceScoreDto result = await connection.QuerySingleOrDefaultAsync<ComplianceScoreDto>(sql, new { start, end, tenantId }, commandTimeout: 300);
        return result ?? new ComplianceScoreDto { ComplianceScore = 0 };
    }

    public async Task<RiskLevelDto> GetCurrentRiskLevelAsync(Guid tenantId, DateTime start, DateTime end, CancellationToken cancellationToken = default)
    {
        string sql = @"
WITH risk_calculation AS (
  SELECT 
    COUNT(*) as total_threats,
    COUNT(DISTINCT te.""SourceAddress"") as unique_sources,
    COUNT(CASE WHEN te.""Category"" LIKE '%critical%' OR te.""Category"" LIKE '%malware%' THEN 1 END) as critical_threats
  FROM ""ThreatEvents"" te
  WHERE te.""DeletedAt"" IS NULL 
    AND te.""Timestamp"" > NOW() - INTERVAL '24 hours'
    AND te.""AsnRegistryId"" IN (SELECT ""AsnRegistryId"" FROM ""TenantAsnRegistries"" WHERE ""TenantId"" = @tenantId AND ""DeletedAt"" IS NULL)
)
SELECT 
  CASE 
    WHEN critical_threats > 100 OR unique_sources > 500 THEN 9
    WHEN critical_threats > 50 OR unique_sources > 250 THEN 7
    WHEN critical_threats > 20 OR unique_sources > 100 THEN 5
    WHEN critical_threats > 10 OR unique_sources > 50 THEN 3
    ELSE 1
  END as RiskLevel
FROM risk_calculation
";
        using IDbConnection connection = CreateConnection();
        RiskLevelDto result = await connection.QuerySingleOrDefaultAsync<RiskLevelDto>(sql, new { start, end, tenantId }, commandTimeout: 300);
        return result ?? new RiskLevelDto { RiskLevel = 1 };
    }

    public async Task<List<RegionalRiskDto>> GetRegionalRiskAsync(Guid tenantId, DateTime start, DateTime end, int limit = 15, CancellationToken cancellationToken = default)
    {
        string sql = @"
SELECT 
  c.""Name"" as TopRiskCountries,
  COUNT(*) as ThreatEvents,
  COUNT(DISTINCT te.""SourceAddress"") as UniqueSources,
  COUNT(DISTINCT te.""Category"") as AttackTypes,
  ROUND((COUNT(*) * COUNT(DISTINCT te.""SourceAddress""))::numeric / 1000, 2) as RiskScore
FROM ""ThreatEvents"" te
JOIN ""Countries"" c ON te.""SourceCountryId"" = c.""Id""
WHERE te.""DeletedAt"" IS NULL 
  AND te.""Timestamp"" BETWEEN @start AND @end
  AND te.""AsnRegistryId"" IN (SELECT ""AsnRegistryId"" FROM ""TenantAsnRegistries"" WHERE ""TenantId"" = @tenantId AND ""DeletedAt"" IS NULL)
GROUP BY c.""Name""
ORDER BY RiskScore DESC
LIMIT @limit
";
        using IDbConnection connection = CreateConnection();
        IEnumerable<RegionalRiskDto> result = await connection.QueryAsync<RegionalRiskDto>(sql, new { start, end, tenantId, limit }, commandTimeout: 300);
        return result.AsList();
    }

    public async Task<List<AttackCategoryDto>> GetAttackCategoryAnalysisAsync(Guid tenantId, DateTime start, DateTime end, int limit = 15, CancellationToken cancellationToken = default)
    {
        string sql = @"
SELECT 
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
  AND te.""AsnRegistryId"" IN (SELECT ""AsnRegistryId"" FROM ""TenantAsnRegistries"" WHERE ""TenantId"" = @tenantId AND ""DeletedAt"" IS NULL)
GROUP BY te.""Category""
ORDER BY TotalEvents DESC
LIMIT @limit
";
        using IDbConnection connection = CreateConnection();
        IEnumerable<AttackCategoryDto> result = await connection.QueryAsync<AttackCategoryDto>(sql, new { start, end, tenantId, limit }, commandTimeout: 300);
        return result.AsList();
    }
}