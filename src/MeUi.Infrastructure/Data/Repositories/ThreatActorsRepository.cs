using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatActors;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace MeUi.Infrastructure.Data.Repositories;

public class ThreatActorsRepository : IThreatActorsRepository
{
  private readonly string _connectionString;

  public ThreatActorsRepository(IConfiguration configuration)
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

  public async Task<List<ActorProfileDto>> GetActorProfilesAsync(DateTime start, DateTime end, int limit = 30, CancellationToken cancellationToken = default)
  {
    string sql = @"
WITH actor_profiles AS (
  SELECT 
    ""SourceAddress"",
    c.""Name"" as source_country,
    ar.""Number"" as asn,
    COUNT(*) as total_events,
    COUNT(DISTINCT ""DestinationPort"") as ports_targeted,
    COUNT(DISTINCT ""Category"") as attack_categories,
    COUNT(DISTINCT ""DestinationAddress"") as targets,
    COUNT(DISTINCT te.""MalwareFamilyId"") as malware_families,
    MIN(""Timestamp"") as first_seen,
    MAX(""Timestamp"") as last_seen,
    EXTRACT(EPOCH FROM (MAX(""Timestamp"") - MIN(""Timestamp""))) / 86400 as campaign_days
  FROM ""ThreatEvents"" te
  JOIN ""Countries"" c ON te.""SourceCountryId"" = c.""Id""
  JOIN ""AsnRegistries"" ar ON te.""AsnRegistryId"" = ar.""Id""
  WHERE te.""DeletedAt"" IS NULL 
    AND te.""Timestamp"" BETWEEN @start AND @end
  GROUP BY ""SourceAddress"", c.""Name"", ar.""Number""
  HAVING COUNT(*) > 20
)
SELECT 
  CAST(""SourceAddress"" as TEXT) as ""ActorIp"",
  source_country as ""OriginCountry"",
  CAST(asn as TEXT) as ""Asn"",
  total_events as ""TotalAttacks"",
  ports_targeted as ""PortsTargeted"",
  attack_categories as ""AttackTypes"",
  targets as ""UniqueTargets"",
  malware_families as ""MalwareUsed"",
  ROUND(campaign_days::numeric, 1) as ""CampaignDurationDays"",
  (total_events * ports_targeted * attack_categories + targets * malware_families) as ""ActorThreatScore"",
  first_seen as ""FirstSeen"",
  last_seen as ""LastSeen""
FROM actor_profiles
ORDER BY ""ActorThreatScore"" DESC
LIMIT @limit";

    using IDbConnection connection = CreateConnection();
    IEnumerable<ActorProfileDto> result = await connection.QueryAsync<ActorProfileDto>(sql, new { start, end, limit }, commandTimeout: 300);
    return result.AsList();
  }

  public async Task<List<ActorCountryDistributionDto>> GetActorDistributionByCountryAsync(DateTime start, DateTime end, int limit = 15, CancellationToken cancellationToken = default)
  {
    string sql = @"
WITH actor_countries AS (
  SELECT 
    c.""Name"" as ""Country"",
    COUNT(DISTINCT ""SourceAddress"") as ""UniqueActors""
  FROM ""ThreatEvents"" te
  JOIN ""Countries"" c ON te.""SourceCountryId"" = c.""Id""
  WHERE te.""DeletedAt"" IS NULL 
    AND te.""Timestamp"" BETWEEN @start AND @end
  GROUP BY c.""Name""
  HAVING COUNT(DISTINCT ""SourceAddress"") > 5
)
SELECT ""Country"", ""UniqueActors""
FROM actor_countries
ORDER BY ""UniqueActors"" DESC
LIMIT @limit";

    using IDbConnection connection = CreateConnection();
    IEnumerable<ActorCountryDistributionDto> result = await connection.QueryAsync<ActorCountryDistributionDto>(sql, new { start, end, limit }, commandTimeout: 300);
    return result.AsList();
  }

  public async Task<List<ActorAsnDto>> GetActorAsnAsync(DateTime start, DateTime end, int limit = 15, CancellationToken cancellationToken = default)
  {
    string sql = @"
WITH actor_asn AS (
  SELECT 
    ar.""Number"" as ""ASN"",
    COUNT(DISTINCT ""SourceAddress"") as ""Actors""
  FROM ""ThreatEvents"" te
  JOIN ""AsnRegistries"" ar ON te.""AsnRegistryId"" = ar.""Id""
  WHERE te.""DeletedAt"" IS NULL 
    AND te.""Timestamp"" BETWEEN @start AND @end
  GROUP BY ar.""Number""
  HAVING COUNT(DISTINCT ""SourceAddress"") > 3
)
SELECT 
  CAST(""ASN"" as TEXT) as ""Asn"",
  ""Actors""
FROM actor_asn
ORDER BY ""Actors"" DESC
LIMIT @limit";

    using IDbConnection connection = CreateConnection();
    IEnumerable<ActorAsnDto> result = await connection.QueryAsync<ActorAsnDto>(sql, new { start, end, limit }, commandTimeout: 300);
    return result.AsList();
  }

  public async Task<List<ActorActivityTimelineDto>> GetTopActorsActivityTimelineAsync(DateTime start, DateTime end, TimeSpan interval, int limit = 10, CancellationToken cancellationToken = default)
  {
    // match the overview pattern for interval grouping
    string intervalStr = interval.TotalDays >= 1 ? "day" : interval.TotalHours >= 1 ? "hour" : "minute";
    string sql = $@"
WITH top_actors AS (
  SELECT ""SourceAddress""
  FROM ""ThreatEvents""
  WHERE ""DeletedAt"" IS NULL 
    AND ""Timestamp"" BETWEEN @start AND @end
  GROUP BY ""SourceAddress""
  HAVING COUNT(*) > 100
  ORDER BY COUNT(*) DESC
  LIMIT @limit
)
SELECT 
  date_trunc('{intervalStr}', ""Timestamp"") as Time,
  CAST(""SourceAddress"" as TEXT) as ""ActorIp"",
  COUNT(*) as Activity
FROM ""ThreatEvents"" te
WHERE te.""DeletedAt"" IS NULL 
  AND te.""Timestamp"" BETWEEN @start AND @end
  AND te.""SourceAddress"" IN (SELECT ""SourceAddress"" FROM top_actors)
GROUP BY Time, te.""SourceAddress""
ORDER BY Time";

    using IDbConnection connection = CreateConnection();
    IEnumerable<ActorActivityTimelineDto> result = await connection.QueryAsync<ActorActivityTimelineDto>(sql, new { start, end, limit }, commandTimeout: 300);
    return result.AsList();
  }

  public async Task<List<ActorTtpDto>> GetActorTtpAnalysisAsync(DateTime start, DateTime end, int limit = 25, CancellationToken cancellationToken = default)
  {
    string sql = @"
WITH actor_ttp AS (
  SELECT 
    ""SourceAddress"",
    CASE 
      WHEN ""Category"" LIKE '%scan%' OR ""Category"" LIKE '%recon%' THEN 'Reconnaissance'
      WHEN ""Category"" LIKE '%brute%' OR ""Category"" LIKE '%login%' THEN 'Credential Access'
      WHEN ""Category"" LIKE '%malware%' OR ""Category"" LIKE '%trojan%' THEN 'Malware Deployment'
      WHEN ""Category"" LIKE '%ddos%' OR ""Category"" LIKE '%dos%' THEN 'Impact/DoS'
      WHEN ""Category"" LIKE '%exploit%' OR ""Category"" LIKE '%vulnerability%' THEN 'Exploitation'
      ELSE 'Other'
    END as ttp_category,
    COUNT(*) as events
  FROM ""ThreatEvents""
  WHERE ""DeletedAt"" IS NULL 
    AND ""Timestamp"" BETWEEN @start AND @end
  GROUP BY ""SourceAddress"", ttp_category
  HAVING COUNT(*) > 10
),
actor_ttp_profile AS (
  SELECT 
    ""SourceAddress"",
    STRING_AGG(ttp_category || ' (' || events || ')', ', ' ORDER BY events DESC) as ttp_profile,
    COUNT(DISTINCT ttp_category) as ttp_diversity,
    SUM(events) as total_events
  FROM actor_ttp
  GROUP BY ""SourceAddress""
)
SELECT 
  CAST(""SourceAddress"" as TEXT) as ""ActorIp"",
  ttp_profile as ""TtpProfile"",
  ttp_diversity as ""TtpDiversity"",
  total_events as ""TotalEvents""
FROM actor_ttp_profile
ORDER BY ttp_diversity DESC, total_events DESC
LIMIT @limit";

    using IDbConnection connection = CreateConnection();
    IEnumerable<ActorTtpDto> result = await connection.QueryAsync<ActorTtpDto>(sql, new { start, end, limit }, commandTimeout: 300);
    return result.AsList();
  }

  public async Task<List<ActorSimilarityDto>> GetActorSimilarityAsync(DateTime start, DateTime end, int limit = 25, CancellationToken cancellationToken = default)
  {
    string sql = @"
WITH actor_profiles AS (
  SELECT 
    te.""SourceAddress"",
    COUNT(*) as total_events,
    COUNT(DISTINCT te.""Category"") as attack_types,
    COUNT(DISTINCT te.""DestinationPort"") as ports_used,
    COUNT(DISTINCT COALESCE(mf.""Name"", 'Unknown')) as malware_families
  FROM ""ThreatEvents"" te
  LEFT JOIN ""MalwareFamilies"" mf ON te.""MalwareFamilyId"" = mf.""Id""
  WHERE te.""DeletedAt"" IS NULL 
    AND te.""Timestamp"" BETWEEN @start AND @end
  GROUP BY te.""SourceAddress""
  HAVING COUNT(*) > 15
),
actor_similarity AS (
  SELECT 
    a1.""SourceAddress"" as actor1,
    a2.""SourceAddress"" as actor2,
    a1.attack_types as a1_attack_types,
    a2.attack_types as a2_attack_types,
    a1.ports_used as a1_ports,
    a2.ports_used as a2_ports,
    a1.malware_families as a1_malware,
    a2.malware_families as a2_malware,
    LEAST(a1.attack_types, a2.attack_types) as common_attack_types,
    LEAST(a1.ports_used, a2.ports_used) as common_ports,
    LEAST(a1.malware_families, a2.malware_families) as common_malware,
    (LEAST(a1.attack_types, a2.attack_types) + LEAST(a1.ports_used, a2.ports_used) + LEAST(a1.malware_families, a2.malware_families)) as similarity_score
  FROM actor_profiles a1
  JOIN actor_profiles a2 ON a1.""SourceAddress"" < a2.""SourceAddress""
  WHERE a1.attack_types > 1 AND a2.attack_types > 1
)
SELECT 
  CAST(actor1 as TEXT) as ""Actor1"",
  CAST(actor2 as TEXT) as ""Actor2"",
  a1_attack_types as ""A1AttackTypes"",
  a2_attack_types as ""A2AttackTypes"",
  a1_ports as ""A1Ports"",
  a2_ports as ""A2Ports"",
  a1_malware as ""A1Malware"",
  a2_malware as ""A2Malware"",
  common_attack_types as ""CommonAttackTypes"",
  common_ports as ""CommonPorts"",
  common_malware as ""CommonMalware"",
  similarity_score as ""SimilarityScore""
FROM actor_similarity
WHERE similarity_score > 2
ORDER BY similarity_score DESC
LIMIT @limit";

    using IDbConnection connection = CreateConnection();
    IEnumerable<ActorSimilarityDto> result = await connection.QueryAsync<ActorSimilarityDto>(sql, new { start, end, limit }, commandTimeout: 300);
    return result.AsList();
  }

  public async Task<List<ActorPersistenceDto>> GetActorPersistenceAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default)
  {
    string sql = @"
WITH actor_persistence AS (
  SELECT 
    ""SourceAddress"",
    EXTRACT(EPOCH FROM (MAX(""Timestamp"") - MIN(""Timestamp""))) / 86400 as campaign_days,
    COUNT(*) as total_events
  FROM ""ThreatEvents"" 
  WHERE ""DeletedAt"" IS NULL 
    AND ""Timestamp"" BETWEEN @start AND @end
  GROUP BY ""SourceAddress""
  HAVING COUNT(*) > 10
),
persistence_categories AS (
  SELECT 
    CASE 
      WHEN campaign_days < 1 THEN 'Short-term (< 1 day)'
      WHEN campaign_days < 7 THEN 'Medium-term (1-7 days)'
      WHEN campaign_days < 30 THEN 'Long-term (1-4 weeks)'
      ELSE 'Persistent (> 1 month)'
    END as ""Persistence Type"",
    COUNT(*) as ""Actors""
  FROM actor_persistence
  GROUP BY 
    CASE 
      WHEN campaign_days < 1 THEN 'Short-term (< 1 day)'
      WHEN campaign_days < 7 THEN 'Medium-term (1-7 days)'
      WHEN campaign_days < 30 THEN 'Long-term (1-4 weeks)'
      ELSE 'Persistent (> 1 month)'
    END
)
SELECT 
  ""Persistence Type"" as ""PersistenceType"",
  ""Actors""
FROM persistence_categories
ORDER BY ""Actors"" DESC";

    using IDbConnection connection = CreateConnection();
    IEnumerable<ActorPersistenceDto> result = await connection.QueryAsync<ActorPersistenceDto>(sql, new { start, end }, commandTimeout: 300);
    return result.AsList();
  }

  public async Task<List<ActorEvolutionDto>> GetActorEvolutionAsync(DateTime start, DateTime end, int limit = 25, CancellationToken cancellationToken = default)
  {
    string sql = @"
WITH actor_evolution AS (
  SELECT 
    ""SourceAddress"",
    DATE_TRUNC('day', ""Timestamp"") as attack_day,
    COUNT(*) as daily_events,
    COUNT(DISTINCT ""DestinationPort"") as daily_ports,
    COUNT(DISTINCT ""Category"") as daily_categories
  FROM ""ThreatEvents""
  WHERE ""DeletedAt"" IS NULL 
    AND ""Timestamp"" BETWEEN @start AND @end
  GROUP BY ""SourceAddress"", DATE_TRUNC('day', ""Timestamp"")
  HAVING COUNT(*) > 5
),
evolution_metrics AS (
  SELECT 
    ""SourceAddress"",
    COUNT(DISTINCT attack_day) as active_days,
    AVG(daily_events) as avg_daily_events,
    MAX(daily_events) as peak_daily_events,
    AVG(daily_ports) as avg_ports_per_day,
    AVG(daily_categories) as avg_categories_per_day,
    STDDEV(daily_events) as event_variance
  FROM actor_evolution
  GROUP BY ""SourceAddress""
  HAVING COUNT(DISTINCT attack_day) > 2
)
SELECT 
  CAST(""SourceAddress"" as TEXT) as ""ActorIp"",
  active_days as ""ActiveDays"",
  ROUND(avg_daily_events::numeric, 1) as ""AvgEventsPerDay"",
  peak_daily_events as ""PeakEventsPerDay"",
  ROUND(avg_ports_per_day::numeric, 1) as ""AvgPortsPerDay"",
  ROUND(avg_categories_per_day::numeric, 1) as ""AvgCategoriesPerDay"",
  ROUND(COALESCE(event_variance, 0)::numeric, 1) as ""EventVariance""
FROM evolution_metrics
ORDER BY ""PeakEventsPerDay"" DESC
LIMIT @limit";

    using IDbConnection connection = CreateConnection();
    IEnumerable<ActorEvolutionDto> result = await connection.QueryAsync<ActorEvolutionDto>(sql, new { start, end, limit }, commandTimeout: 300);
    return result.AsList();
  }
}
