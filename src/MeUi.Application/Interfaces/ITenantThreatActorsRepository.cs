using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MeUi.Application.Models.ThreatActors;

namespace MeUi.Application.Interfaces;

public interface ITenantThreatActorsRepository
{
    Task<List<ActorProfileDto>> GetActorProfilesAsync(Guid tenantId, DateTime start, DateTime end, int limit = 30, CancellationToken cancellationToken = default);
    Task<List<ActorCountryDistributionDto>> GetActorDistributionByCountryAsync(Guid tenantId, DateTime start, DateTime end, int limit = 15, CancellationToken cancellationToken = default);
    Task<List<ActorAsnDto>> GetActorAsnAsync(Guid tenantId, DateTime start, DateTime end, int limit = 15, CancellationToken cancellationToken = default);
    Task<List<ActorActivityTimelineDto>> GetTopActorsActivityTimelineAsync(Guid tenantId, DateTime start, DateTime end, TimeSpan interval, int limit = 10, CancellationToken cancellationToken = default);
    Task<List<ActorTtpDto>> GetActorTtpAnalysisAsync(Guid tenantId, DateTime start, DateTime end, int limit = 25, CancellationToken cancellationToken = default);
    Task<List<ActorSimilarityDto>> GetActorSimilarityAsync(Guid tenantId, DateTime start, DateTime end, int limit = 25, CancellationToken cancellationToken = default);
    Task<List<ActorPersistenceDto>> GetActorPersistenceAsync(Guid tenantId, DateTime start, DateTime end, CancellationToken cancellationToken = default);
    Task<List<ActorEvolutionDto>> GetActorEvolutionAsync(Guid tenantId, DateTime start, DateTime end, int limit = 25, CancellationToken cancellationToken = default);
}