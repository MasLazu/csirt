using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MeUi.Application.Models.ThreatActors;

namespace MeUi.Application.Interfaces;

public interface IThreatActorsRepository
{
    Task<List<ActorProfileDto>> GetActorProfilesAsync(DateTime start, DateTime end, int limit = 30, CancellationToken cancellationToken = default);
    Task<List<ActorCountryDistributionDto>> GetActorDistributionByCountryAsync(DateTime start, DateTime end, int limit = 15, CancellationToken cancellationToken = default);
    Task<List<ActorAsnDto>> GetActorAsnAsync(DateTime start, DateTime end, int limit = 15, CancellationToken cancellationToken = default);
    Task<List<ActorActivityTimelineDto>> GetTopActorsActivityTimelineAsync(DateTime start, DateTime end, TimeSpan interval, int limit = 10, CancellationToken cancellationToken = default);
    Task<List<ActorTtpDto>> GetActorTtpAnalysisAsync(DateTime start, DateTime end, int limit = 25, CancellationToken cancellationToken = default);
    Task<List<ActorSimilarityDto>> GetActorSimilarityAsync(DateTime start, DateTime end, int limit = 25, CancellationToken cancellationToken = default);
    Task<List<ActorPersistenceDto>> GetActorPersistenceAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default);
    Task<List<ActorEvolutionDto>> GetActorEvolutionAsync(DateTime start, DateTime end, int limit = 25, CancellationToken cancellationToken = default);
}
