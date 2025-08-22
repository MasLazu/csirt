using MeUi.Application.Models.ThreatNetwork;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Interfaces;

public interface ITenantThreatNetworkRepository
{
    Task<List<TargetedPortDto>> GetMostTargetedPortsAsync(Guid tenantId, DateTime start, DateTime end, int limit = 15, CancellationToken cancellationToken = default);
    Task<List<ProtocolDistributionDto>> GetProtocolDistributionAsync(Guid tenantId, DateTime start, DateTime end, CancellationToken cancellationToken = default);
    Task<List<HighRiskIpDto>> GetHighRiskIpReputationAsync(Guid tenantId, DateTime start, DateTime end, int limit = 20, CancellationToken cancellationToken = default);
    Task<List<CriticalPortTimePointDto>> GetCriticalPortTimelineAsync(Guid tenantId, DateTime start, DateTime end, TimeSpan interval, CancellationToken cancellationToken = default);
    Task<List<AsnNetworkDto>> GetAsnNetworkAnalysisAsync(Guid tenantId, DateTime start, DateTime end, int limit = 20, CancellationToken cancellationToken = default);
    Task<List<TargetedInfrastructureDto>> GetMostTargetedInfrastructureAsync(Guid tenantId, DateTime start, DateTime end, int limit = 20, CancellationToken cancellationToken = default);
    Task<List<ProtocolTrendDto>> GetProtocolTrendsAsync(Guid tenantId, DateTime start, DateTime end, TimeSpan interval, CancellationToken cancellationToken = default);
}