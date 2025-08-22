using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatNetwork;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace MeUi.Infrastructure.Data.Repositories;

public class TenantThreatNetworkRepository : ITenantThreatNetworkRepository
{
    private readonly string _connectionString;

    public TenantThreatNetworkRepository(IConfiguration configuration)
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

    public async Task<List<TargetedPortDto>> GetMostTargetedPortsAsync(Guid tenantId, DateTime start, DateTime end, int limit = 15, CancellationToken cancellationToken = default)
    {
        var sql = @"
SELECT 
  CAST(""DestinationPort"" as TEXT) as Port,
  COUNT(*) as Attacks,
  COUNT(DISTINCT ""SourceAddress"") as UniqueSources
FROM ""ThreatEvents"" te
WHERE te.""DeletedAt"" IS NULL 
  AND te.""Timestamp"" BETWEEN @start AND @end
  AND ""DestinationPort"" IS NOT NULL
  AND te.""AsnRegistryId"" IN (SELECT ""AsnRegistryId"" FROM ""TenantAsnRegistries"" WHERE ""TenantId"" = @tenantId AND ""DeletedAt"" IS NULL)
GROUP BY ""DestinationPort""
ORDER BY Attacks DESC
LIMIT @limit";

        using var connection = CreateConnection();
        var result = await connection.QueryAsync<TargetedPortDto>(sql, new { start, end, tenantId, limit }, commandTimeout: 300);
        return result.AsList();
    }

    public async Task<List<ProtocolDistributionDto>> GetProtocolDistributionAsync(Guid tenantId, DateTime start, DateTime end, CancellationToken cancellationToken = default)
    {
        var sql = @"
SELECT 
  p.""Name"" as Protocol,
  COUNT(*) as Events
FROM ""ThreatEvents"" te
JOIN ""Protocols"" p ON te.""ProtocolId"" = p.""Id""
WHERE te.""DeletedAt"" IS NULL 
  AND te.""Timestamp"" BETWEEN @start AND @end
  AND te.""AsnRegistryId"" IN (SELECT ""AsnRegistryId"" FROM ""TenantAsnRegistries"" WHERE ""TenantId"" = @tenantId AND ""DeletedAt"" IS NULL)
GROUP BY p.""Name""
ORDER BY Events DESC";

        using var connection = CreateConnection();
        var result = await connection.QueryAsync<ProtocolDistributionDto>(sql, new { start, end, tenantId }, commandTimeout: 300);
        return result.AsList();
    }

    public async Task<List<HighRiskIpDto>> GetHighRiskIpReputationAsync(Guid tenantId, DateTime start, DateTime end, int limit = 20, CancellationToken cancellationToken = default)
    {
        var sql = @"
SELECT 
  CAST(te.""SourceAddress"" as TEXT) as IpAddress,
  COUNT(*) as TotalAttacks,
  COUNT(DISTINCT te.""DestinationPort"") as PortsTargeted,
  COUNT(DISTINCT te.""Category"") as AttackTypes,
  COUNT(DISTINCT p.""Name"") as ProtocolsUsed,
  (COUNT(*) * COUNT(DISTINCT te.""DestinationPort"") * COUNT(DISTINCT te.""Category"")) as AttackScore,
  MIN(te.""Timestamp"") as FirstSeen,
  MAX(te.""Timestamp"") as LastSeen
FROM ""ThreatEvents"" te
JOIN ""Protocols"" p ON te.""ProtocolId"" = p.""Id""
WHERE te.""DeletedAt"" IS NULL 
  AND te.""Timestamp"" BETWEEN @start AND @end
  AND te.""AsnRegistryId"" IN (SELECT ""AsnRegistryId"" FROM ""TenantAsnRegistries"" WHERE ""TenantId"" = @tenantId AND ""DeletedAt"" IS NULL)
GROUP BY te.""SourceAddress""
ORDER BY AttackScore DESC
LIMIT @limit";

        using var connection = CreateConnection();
        var result = await connection.QueryAsync<HighRiskIpDto>(sql, new { start, end, tenantId, limit }, commandTimeout: 300);
        return result.AsList();
    }

    public async Task<List<CriticalPortTimePointDto>> GetCriticalPortTimelineAsync(Guid tenantId, DateTime start, DateTime end, TimeSpan interval, CancellationToken cancellationToken = default)
    {
        var intervalStr = interval.TotalDays >= 1 ? "day" : interval.TotalHours >= 1 ? "hour" : "minute";
        var sql = $@"
SELECT 
  date_trunc('{intervalStr}', ""Timestamp"") as Time,
  CAST(""DestinationPort"" as TEXT) as Port,
  COUNT(*) as Attacks
FROM ""ThreatEvents"" te
WHERE te.""DeletedAt"" IS NULL 
  AND te.""Timestamp"" BETWEEN @start AND @end
  AND ""DestinationPort"" IN (22, 23, 80, 443, 3389, 1433, 3306, 5432, 6379, 27017)
  AND te.""AsnRegistryId"" IN (SELECT ""AsnRegistryId"" FROM ""TenantAsnRegistries"" WHERE ""TenantId"" = @tenantId AND ""DeletedAt"" IS NULL)
GROUP BY Time, ""DestinationPort""
ORDER BY Time";

        using var connection = CreateConnection();
        var result = await connection.QueryAsync<CriticalPortTimePointDto>(sql, new { start, end, tenantId }, commandTimeout: 300);
        return result.AsList();
    }

    public async Task<List<AsnNetworkDto>> GetAsnNetworkAnalysisAsync(Guid tenantId, DateTime start, DateTime end, int limit = 20, CancellationToken cancellationToken = default)
    {
        var sql = @"
SELECT 
  ar.""Number"" as ASN,
  ar.""Description"" as Organization,
  COUNT(*) as Events,
  COUNT(DISTINCT ""SourceAddress"") as UniqueIps,
  COUNT(DISTINCT ""DestinationPort"") as PortsTargeted,
  COUNT(DISTINCT ""Category"") as AttackTypes
FROM ""ThreatEvents"" te
JOIN ""AsnRegistries"" ar ON te.""AsnRegistryId"" = ar.""Id""
WHERE te.""DeletedAt"" IS NULL 
  AND te.""Timestamp"" BETWEEN @start AND @end
  AND te.""AsnRegistryId"" IN (SELECT ""AsnRegistryId"" FROM ""TenantAsnRegistries"" WHERE ""TenantId"" = @tenantId AND ""DeletedAt"" IS NULL)
GROUP BY ar.""Number"", ar.""Description""
ORDER BY Events DESC
LIMIT @limit";

        using var connection = CreateConnection();
        var result = await connection.QueryAsync<AsnNetworkDto>(sql, new { start, end, tenantId, limit }, commandTimeout: 300);
        return result.AsList();
    }

    public async Task<List<TargetedInfrastructureDto>> GetMostTargetedInfrastructureAsync(Guid tenantId, DateTime start, DateTime end, int limit = 20, CancellationToken cancellationToken = default)
    {
        var sql = @"
SELECT 
  CAST(""DestinationAddress"" as TEXT) as TargetIp,
  COUNT(*) as AttacksReceived,
  COUNT(DISTINCT ""SourceAddress"") as UniqueAttackers,
  COUNT(DISTINCT ""DestinationPort"") as PortsAttacked,
  COUNT(DISTINCT ""Category"") as AttackTypes,
  MIN(""Timestamp"") as FirstAttack,
  MAX(""Timestamp"") as LastAttack
FROM ""ThreatEvents"" te
WHERE te.""DeletedAt"" IS NULL 
  AND te.""Timestamp"" BETWEEN @start AND @end
  AND ""DestinationAddress"" IS NOT NULL
  AND te.""AsnRegistryId"" IN (SELECT ""AsnRegistryId"" FROM ""TenantAsnRegistries"" WHERE ""TenantId"" = @tenantId AND ""DeletedAt"" IS NULL)
GROUP BY ""DestinationAddress""
ORDER BY AttacksReceived DESC
LIMIT @limit";

        using var connection = CreateConnection();
        var result = await connection.QueryAsync<TargetedInfrastructureDto>(sql, new { start, end, tenantId, limit }, commandTimeout: 300);
        return result.AsList();
    }

    public async Task<List<ProtocolTrendDto>> GetProtocolTrendsAsync(Guid tenantId, DateTime start, DateTime end, TimeSpan interval, CancellationToken cancellationToken = default)
    {
        var intervalStr = interval.TotalDays >= 1 ? "day" : interval.TotalHours >= 1 ? "hour" : "minute";
        var sql = $@"
SELECT 
  date_trunc('{intervalStr}', te.""Timestamp"") as Time,
  p.""Name"" as Protocol,
  COUNT(*) as Events
FROM ""ThreatEvents"" te
JOIN ""Protocols"" p ON te.""ProtocolId"" = p.""Id""
WHERE te.""DeletedAt"" IS NULL 
  AND te.""Timestamp"" BETWEEN @start AND @end
  AND te.""AsnRegistryId"" IN (SELECT ""AsnRegistryId"" FROM ""TenantAsnRegistries"" WHERE ""TenantId"" = @tenantId AND ""DeletedAt"" IS NULL)
GROUP BY Time, p.""Name""
ORDER BY Time";

        using var connection = CreateConnection();
        var result = await connection.QueryAsync<ProtocolTrendDto>(sql, new { start, end, tenantId }, commandTimeout: 300);
        return result.AsList();
    }
}