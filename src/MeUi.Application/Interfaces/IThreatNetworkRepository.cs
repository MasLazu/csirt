using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MeUi.Application.Models.ThreatNetwork;

namespace MeUi.Application.Interfaces;

public interface IThreatNetworkRepository
{
    Task<List<TargetedPortDto>> GetMostTargetedPortsAsync(DateTime start, DateTime end, int limit = 15, CancellationToken cancellationToken = default);
    Task<List<ProtocolDistributionDto>> GetProtocolDistributionAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default);
    Task<List<HighRiskIpDto>> GetHighRiskIpReputationAsync(DateTime start, DateTime end, int limit = 20, CancellationToken cancellationToken = default);
    Task<List<AsnNetworkDto>> GetAsnNetworkAnalysisAsync(DateTime start, DateTime end, int limit = 20, CancellationToken cancellationToken = default);
    Task<List<CriticalPortTimePointDto>> GetCriticalPortTimelineAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default);
    Task<List<TargetedInfrastructureDto>> GetMostTargetedInfrastructureAsync(DateTime start, DateTime end, int limit = 20, CancellationToken cancellationToken = default);
    Task<List<ProtocolTrendDto>> GetProtocolTrendsAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default);
}
