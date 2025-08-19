using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MeUi.Application.Models.ThreatCompliance;

namespace MeUi.Application.Interfaces
{
    public interface IThreatComplianceRepository
    {
        Task<List<ExecutiveSummaryDto>> GetExecutiveSummaryAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default);
        Task<List<KpiTrendPointDto>> GetKpiTrendAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default);
        Task<ComplianceScoreDto> GetComplianceScoreAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default);
        Task<RiskLevelDto> GetCurrentRiskLevelAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default);
        Task<List<RegionalRiskDto>> GetRegionalRiskAsync(DateTime start, DateTime end, int limit = 15, CancellationToken cancellationToken = default);
        Task<List<AttackCategoryDto>> GetAttackCategoryAnalysisAsync(DateTime start, DateTime end, int limit = 15, CancellationToken cancellationToken = default);
    }
}
