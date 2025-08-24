using System.Data;
using Dapper;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatNetwork;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace MeUi.Infrastructure.Data.Repositories;

public class ThreatNetworkRepository : IThreatNetworkRepository
{
  private readonly string _connectionString;

  public ThreatNetworkRepository(IConfiguration configuration)
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

  public async Task<List<TargetedPortDto>> GetMostTargetedPortsAsync(DateTime start, DateTime end, int limit = 15, CancellationToken cancellationToken = default)
  {
        string sql = @"SELECT 
  CAST(""DestinationPort"" as TEXT) as ""Port"",
  COUNT(*) as ""Attacks"",
  COUNT(DISTINCT ""SourceAddress"") as ""UniqueSources""
FROM ""ThreatEvents"" 
WHERE ""DeletedAt"" IS NULL 
  AND ""Timestamp"" BETWEEN @start AND @end
  AND ""DestinationPort"" IS NOT NULL
GROUP BY ""DestinationPort""
ORDER BY ""Attacks"" DESC
LIMIT @limit";

    using IDbConnection connection = CreateConnection();
        IEnumerable<TargetedPortDto> result = await connection.QueryAsync<TargetedPortDto>(sql, new { start, end, limit }, commandTimeout: 300);
    return result.AsList();
  }

  public async Task<List<ProtocolDistributionDto>> GetProtocolDistributionAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default)
  {
        string sql = @"SELECT 
  p.""Name"" as ""Protocol"",
  COUNT(*) as ""Events""
FROM ""ThreatEvents"" te
JOIN ""Protocols"" p ON te.""ProtocolId"" = p.""Id""
WHERE te.""DeletedAt"" IS NULL 
  AND te.""Timestamp"" BETWEEN @start AND @end
GROUP BY p.""Name""
ORDER BY ""Events"" DESC";

    using IDbConnection connection = CreateConnection();
        IEnumerable<ProtocolDistributionDto> result = await connection.QueryAsync<ProtocolDistributionDto>(sql, new { start, end }, commandTimeout: 300);
    return result.AsList();
  }

  public async Task<List<HighRiskIpDto>> GetHighRiskIpReputationAsync(DateTime start, DateTime end, int limit = 20, CancellationToken cancellationToken = default)
  {
        string sql = @"SELECT 
  te.""SourceAddress""::text as ""IpAddress"",
  COUNT(*) as ""TotalAttacks"",
  COUNT(DISTINCT te.""DestinationPort"") as ""PortsTargeted"",
  COUNT(DISTINCT te.""Category"") as ""AttackTypes"",
  COUNT(DISTINCT p.""Name"") as ""ProtocolsUsed"",
  (COUNT(*) * COUNT(DISTINCT te.""DestinationPort"") * COUNT(DISTINCT te.""Category"")) as ""AttackScore"",
  MIN(te.""Timestamp"") as ""FirstSeen"",
  MAX(te.""Timestamp"") as ""LastSeen""
FROM ""ThreatEvents"" te
JOIN ""Protocols"" p ON te.""ProtocolId"" = p.""Id""
WHERE te.""DeletedAt"" IS NULL 
  AND te.""Timestamp"" BETWEEN @start AND @end
GROUP BY te.""SourceAddress""
ORDER BY ""AttackScore"" DESC
LIMIT @limit";

    using IDbConnection connection = CreateConnection();
        IEnumerable<HighRiskIpDto> result = await connection.QueryAsync<HighRiskIpDto>(sql, new { start, end, limit }, commandTimeout: 300);
    return result.AsList();
  }

  public async Task<List<AsnNetworkDto>> GetAsnNetworkAnalysisAsync(DateTime start, DateTime end, int limit = 20, CancellationToken cancellationToken = default)
  {
        string sql = @"SELECT 
  ar.""Number"" as ""ASN"",
  ar.""Description"" as ""Organization"",
  COUNT(*) as ""Events"",
  COUNT(DISTINCT ""SourceAddress"") as ""UniqueIPs"",
  COUNT(DISTINCT ""DestinationPort"") as ""PortsTargeted"",
  COUNT(DISTINCT ""Category"") as ""AttackTypes""
FROM ""ThreatEvents"" te
JOIN ""AsnRegistries"" ar ON te.""AsnRegistryId"" = ar.""Id""
WHERE te.""DeletedAt"" IS NULL 
  AND te.""Timestamp"" BETWEEN @start AND @end
GROUP BY ar.""Number"", ar.""Description""
ORDER BY ""Events"" DESC
LIMIT @limit";

    using IDbConnection connection = CreateConnection();
        IEnumerable<AsnNetworkDto> result = await connection.QueryAsync<AsnNetworkDto>(sql, new { start, end, limit }, commandTimeout: 300);
    return result.AsList();
  }

  public async Task<List<CriticalPortTimePointDto>> GetCriticalPortTimelineAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default)
  {
        string sql = @"SELECT 
  DATE_TRUNC('hour', ""Timestamp"") as ""Time"",
  CAST(""DestinationPort"" as TEXT) as ""Port"",
  COUNT(*) as ""Attacks""
FROM ""ThreatEvents"" 
WHERE ""DeletedAt"" IS NULL 
  AND ""Timestamp"" BETWEEN @start AND @end
  AND ""DestinationPort"" IN (22, 23, 80, 443, 3389, 1433, 3306, 5432, 6379, 27017)
GROUP BY ""Time"", ""DestinationPort""
ORDER BY ""Time""";

    using IDbConnection connection = CreateConnection();
        IEnumerable<CriticalPortTimePointDto> result = await connection.QueryAsync<CriticalPortTimePointDto>(sql, new { start, end }, commandTimeout: 300);
    return result.AsList();
  }

  public async Task<List<TargetedInfrastructureDto>> GetMostTargetedInfrastructureAsync(DateTime start, DateTime end, int limit = 20, CancellationToken cancellationToken = default)
  {
        string sql = @"SELECT 
  te.""DestinationAddress""::text as ""TargetIp"",
  COUNT(*) as ""AttacksReceived"",
  COUNT(DISTINCT te.""SourceAddress""::text) as ""UniqueAttackers"",
  COUNT(DISTINCT te.""DestinationPort"") as ""PortsAttacked"",
  COUNT(DISTINCT te.""Category"") as ""AttackTypes"",
  MIN(te.""Timestamp"") as ""FirstAttack"",
  MAX(te.""Timestamp"") as ""LastAttack""
FROM ""ThreatEvents"" te
WHERE te.""DeletedAt"" IS NULL 
  AND te.""Timestamp"" BETWEEN @start AND @end
  AND te.""DestinationAddress"" IS NOT NULL
GROUP BY te.""DestinationAddress""::text
ORDER BY ""AttacksReceived"" DESC
LIMIT @limit";

    using IDbConnection connection = CreateConnection();
        IEnumerable<TargetedInfrastructureDto> result = await connection.QueryAsync<TargetedInfrastructureDto>(sql, new { start, end, limit }, commandTimeout: 300);
    return result.AsList();
  }

  public async Task<List<ProtocolTrendDto>> GetProtocolTrendsAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default)
  {
        string sql = @"SELECT 
  DATE_TRUNC('hour', te.""Timestamp"") as time,
  p.""Name"" as ""Protocol"",
  COUNT(*) as ""Events""
FROM ""ThreatEvents"" te
JOIN ""Protocols"" p ON te.""ProtocolId"" = p.""Id""
WHERE te.""DeletedAt"" IS NULL 
  AND te.""Timestamp"" BETWEEN @start AND @end
GROUP BY time, p.""Name""
ORDER BY time";

    using IDbConnection connection = CreateConnection();
        IEnumerable<ProtocolTrendDto> result = await connection.QueryAsync<ProtocolTrendDto>(sql, new { start, end }, commandTimeout: 300);
    return result.AsList();
  }
}
