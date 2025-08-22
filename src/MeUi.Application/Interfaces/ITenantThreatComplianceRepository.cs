using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MeUi.Application.Models.ThreatCompliance;

namespace MeUi.Application.Interfaces;

public interface ITenantThreatComplianceRepository
{
    Task<List<ExecutiveSummaryDto>> GetExecutiveSummaryAsync(Guid tenantId, DateTime start, DateTime end, CancellationToken cancellationToken = default);
    Task<List<KpiTrendPointDto>> GetKpiTrendAsync(Guid tenantId, DateTime start, DateTime end, CancellationToken cancellationToken = default);
    Task<ComplianceScoreDto> GetComplianceScoreAsync(Guid tenantId, DateTime start, DateTime end, CancellationToken cancellationToken = default);
    Task<RiskLevelDto> GetCurrentRiskLevelAsync(Guid tenantId, DateTime start, DateTime end, CancellationToken cancellationToken = default);
    Task<List<RegionalRiskDto>> GetRegionalRiskAsync(Guid tenantId, DateTime start, DateTime end, int limit = 15, CancellationToken cancellationToken = default);
    Task<List<AttackCategoryDto>> GetAttackCategoryAnalysisAsync(Guid tenantId, DateTime start, DateTime end, int limit = 15, CancellationToken cancellationToken = default);
}