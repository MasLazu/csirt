using System.Net;
using Microsoft.EntityFrameworkCore;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;
using MeUi.Infrastructure.Data;

namespace MeUi.Infrastructure.Data.Repositories;

/// <summary>
/// Specialized analytics repository for ThreatEvent entities
/// Optimized for TimescaleDB hypertable operations and threat intelligence analytics
/// </summary>
public class ThreatEventAnalyticsRepository : IThreatEventAnalyticsRepository
{
    private readonly ApplicationDbContext _context;

    public ThreatEventAnalyticsRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    #region Timeline Analytics

    public async Task<IEnumerable<TimelineDataPoint>> GetTimelineAnalyticsAsync(
        DateTime startTime,
        DateTime endTime,
        string timeInterval,
        Guid? tenantId = null,
        string? category = null,
        Guid? malwareFamilyId = null,
        Guid? sourceCountryId = null,
        CancellationToken ct = default)
    {
        // Ensure UTC timestamps for PostgreSQL compatibility
        startTime = DateTime.SpecifyKind(startTime, DateTimeKind.Utc);
        endTime = DateTime.SpecifyKind(endTime, DateTimeKind.Utc);

        IQueryable<ThreatEvent> query = _context.ThreatEvents.AsQueryable();

        // Apply filters
        // TODO: Multi-tenancy - ThreatEvent doesn't have TenantId property
        if (tenantId.HasValue)
        {
            query = query.Where(te => te.AsnRegistry.AsnRegistryTenants.Any(tat => tat.TenantId == tenantId.Value));
        }

        query = query.Where(te => te.Timestamp >= startTime && te.Timestamp <= endTime);

        if (!string.IsNullOrEmpty(category))
        {
            query = query.Where(te => te.Category == category);
        }

        if (malwareFamilyId.HasValue)
        {
            query = query.Where(te => te.MalwareFamilyId == malwareFamilyId.Value);
        }

        if (sourceCountryId.HasValue)
        {
            query = query.Where(te => te.SourceCountryId == sourceCountryId.Value);
        }

        // Use TimescaleDB time_bucket function for optimal performance
        string sql = $@"
            SELECT 
                time_bucket(INTERVAL '{GetIntervalString(timeInterval)}', ""Timestamp"") as ""Timestamp"",
                COUNT(*) as ""Count"",
                COALESCE(""Category"", 'Unknown') as ""Category"",
                5.0 as ""AverageRiskScore"",
                COUNT(DISTINCT ""SourceAddress"") as ""UniqueSourceIps"",
                COUNT(DISTINCT ""DestinationAddress"") as ""UniqueDestinationIps""
            FROM ""ThreatEvents""
            WHERE ""Timestamp"" >= {{0}} AND ""Timestamp"" <= {{1}}
            {(tenantId.HasValue ? "AND \"AsnRegistryId\" IN (SELECT \"AsnRegistryId\" FROM \"TenantAsnRegistries\" WHERE \"TenantId\" = {2})" : "")}
            {(!string.IsNullOrEmpty(category) ? $"AND \"Category\" = '{category}'" : "")}
            {(malwareFamilyId.HasValue ? $"AND \"MalwareFamilyId\" = '{malwareFamilyId}'" : "")}
            {(sourceCountryId.HasValue ? $"AND \"SourceCountryId\" = '{sourceCountryId}'" : "")}
            GROUP BY time_bucket(INTERVAL '{GetIntervalString(timeInterval)}', ""Timestamp""), ""Category""
            ORDER BY ""Timestamp"" ASC";

        object[] parameters = tenantId.HasValue
            ? new object[] { startTime, endTime, tenantId.Value }
            : new object[] { startTime, endTime };

        List<TimelineDataPoint> results = await _context.Database
            .SqlQueryRaw<TimelineDataPoint>(sql, parameters)
            .ToListAsync(ct);

        return results;
    }

    public async Task<IEnumerable<ComparativeTimelineDataPoint>> GetComparativeTimelineAsync(
        DateTime currentStartTime,
        DateTime currentEndTime,
        DateTime comparisonStartTime,
        DateTime comparisonEndTime,
        string timeInterval,
        Guid? tenantId = null,
        CancellationToken ct = default)
    {
        // Ensure UTC timestamps for PostgreSQL compatibility
        currentStartTime = DateTime.SpecifyKind(currentStartTime, DateTimeKind.Utc);
        currentEndTime = DateTime.SpecifyKind(currentEndTime, DateTimeKind.Utc);
        comparisonStartTime = DateTime.SpecifyKind(comparisonStartTime, DateTimeKind.Utc);
        comparisonEndTime = DateTime.SpecifyKind(comparisonEndTime, DateTimeKind.Utc);

        string sql = $@"
            WITH current_period AS (
                SELECT 
                    time_bucket(INTERVAL '{GetIntervalString(timeInterval)}', ""Timestamp"") as ""Timestamp"",
                    COUNT(*) as ""Count"",
                    COALESCE(""Category"", 'Unknown') as ""Category"",
                    COUNT(DISTINCT ""SourceAddress"") as ""UniqueSourceIps"",
                    COUNT(DISTINCT ""DestinationAddress"") as ""UniqueDestinationIps""
                FROM ""ThreatEvents""
                WHERE ""Timestamp"" >= {{0}} AND ""Timestamp"" <= {{1}}
                {(tenantId.HasValue ? "AND \"AsnRegistryId\" IN (SELECT \"AsnRegistryId\" FROM \"TenantAsnRegistries\" WHERE \"TenantId\" = {4})" : "")}
                GROUP BY time_bucket(INTERVAL '{GetIntervalString(timeInterval)}', ""Timestamp""), ""Category""
            ),
            comparison_period AS (
                SELECT 
                    time_bucket(INTERVAL '{GetIntervalString(timeInterval)}', ""Timestamp"") as ""Timestamp"",
                    COUNT(*) as ""Count"",
                    COALESCE(""Category"", 'Unknown') as ""Category""
                FROM ""ThreatEvents""
                WHERE ""Timestamp"" >= {{2}} AND ""Timestamp"" <= {{3}}
                {(tenantId.HasValue ? "AND \"AsnRegistryId\" IN (SELECT \"AsnRegistryId\" FROM \"TenantAsnRegistries\" WHERE \"TenantId\" = {4})" : "")}
                GROUP BY time_bucket(INTERVAL '{GetIntervalString(timeInterval)}', ""Timestamp""), ""Category""
            )
            SELECT 
                cp.""Timestamp"",
                cp.""Count"",
                cp.""Category"",
                5.0 as ""AverageRiskScore"",
                cp.""UniqueSourceIps"",
                cp.""UniqueDestinationIps"",
                COALESCE(comp.""Count"", 0) as ""PreviousPeriodCount"",
                CASE 
                    WHEN COALESCE(comp.""Count"", 0) = 0 THEN 100.0
                    ELSE ((cp.""Count"" - COALESCE(comp.""Count"", 0)) * 100.0 / COALESCE(comp.""Count"", 1))
                END as ""PercentageChange"",
                CASE 
                    WHEN cp.""Count"" > COALESCE(comp.""Count"", 0) THEN 'increasing'
                    WHEN cp.""Count"" < COALESCE(comp.""Count"", 0) THEN 'decreasing'
                    ELSE 'stable'
                END as ""TrendDirection""
            FROM current_period cp
            LEFT JOIN comparison_period comp ON cp.""Category"" = comp.""Category""
            ORDER BY cp.""Timestamp"" ASC";

        object[] parameters = tenantId.HasValue
            ? new object[] { currentStartTime, currentEndTime, comparisonStartTime, comparisonEndTime, tenantId.Value }
            : new object[] { currentStartTime, currentEndTime, comparisonStartTime, comparisonEndTime };

        List<ComparativeTimelineDataPoint> results = await _context.Database
            .SqlQueryRaw<ComparativeTimelineDataPoint>(sql, parameters)
            .ToListAsync(ct);

        return results;
    }

    #endregion

    #region Summary Analytics

    public async Task<ThreatEventSummary> GetSummaryAnalyticsAsync(
        DateTime startTime,
        DateTime endTime,
        Guid? tenantId = null,
        CancellationToken ct = default)
    {
        // Ensure UTC timestamps for PostgreSQL compatibility
        startTime = DateTime.SpecifyKind(startTime, DateTimeKind.Utc);
        endTime = DateTime.SpecifyKind(endTime, DateTimeKind.Utc);

        IQueryable<ThreatEvent> query = _context.ThreatEvents
            .Include(te => te.SourceCountry)
            .Include(te => te.MalwareFamily)
            .Where(te => te.Timestamp >= startTime && te.Timestamp <= endTime);

        // Filter by tenant's ASN registries
        if (tenantId.HasValue)
        {
            query = query.Where(te => _context.TenantAsnRegistries
                .Any(tar => tar.AsnRegistryId == te.AsnRegistryId && tar.TenantId == tenantId.Value));
        }

        List<ThreatEvent> events = await query.ToListAsync(ct);

        double totalHours = (endTime - startTime).TotalHours;
        double avgEventsPerHour = totalHours > 0 ? events.Count / totalHours : 0;

        // Find peak activity
        IGrouping<DateTime, ThreatEvent>? hourlyGroups = events
            .GroupBy(e => new DateTime(e.Timestamp.Year, e.Timestamp.Month, e.Timestamp.Day, e.Timestamp.Hour, 0, 0))
            .OrderByDescending(g => g.Count())
            .FirstOrDefault();

        var categoryDistribution = events
            .GroupBy(e => e.Category)
            .ToDictionary(g => g.Key, g => g.Count());

        return new ThreatEventSummary
        {
            TotalEvents = events.Count,
            UniqueSourceIps = events.Select(e => e.SourceAddress).Distinct().Count(),
            UniqueDestinationIps = events.Where(e => e.DestinationAddress != null)
                .Select(e => e.DestinationAddress).Distinct().Count(),
            UniqueMalwareFamilies = events.Where(e => e.MalwareFamilyId.HasValue)
                .Select(e => e.MalwareFamilyId).Distinct().Count(),
            UniqueCountries = events.Where(e => e.SourceCountryId.HasValue)
                .Select(e => e.SourceCountryId).Distinct().Count(),
            UniqueAsns = events.Select(e => e.AsnRegistryId).Distinct().Count(),
            MostActiveCategory = categoryDistribution.OrderByDescending(kvp => kvp.Value)
                .FirstOrDefault().Key ?? "Unknown",
            MostTargetedCountry = events.Where(e => e.DestinationCountry != null)
                .GroupBy(e => e.DestinationCountry!.Name)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault()?.Key ?? "Unknown",
            MostActiveMalwareFamily = events.Where(e => e.MalwareFamily != null)
                .GroupBy(e => e.MalwareFamily!.Name)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault()?.Key ?? "Unknown",
            AverageEventsPerHour = avgEventsPerHour,
            PeakActivityTime = hourlyGroups?.Key ?? DateTime.MinValue,
            PeakActivityCount = hourlyGroups?.Count() ?? 0,
            CategoryDistribution = categoryDistribution
        };
    }

    public async Task<ThreatEventDashboardMetrics> GetDashboardMetricsAsync(
        Guid? tenantId = null,
        CancellationToken ct = default)
    {
        DateTime now = DateTime.UtcNow;
        DateTime last24Hours = now.AddHours(-24);
        DateTime lastHour = now.AddHours(-1);
        DateTime yesterday = now.AddDays(-1);

        IQueryable<ThreatEvent> query = _context.ThreatEvents.AsQueryable();
        if (tenantId.HasValue)
        {
            query = query.Where(te => _context.TenantAsnRegistries
                .Any(tar => tar.AsnRegistryId == te.AsnRegistryId && tar.TenantId == tenantId.Value));
        }


        int eventsLast24Hours = await query
            .CountAsync(te => te.Timestamp >= last24Hours, ct);

        int eventsLastHour = await query
            .CountAsync(te => te.Timestamp >= lastHour, ct);

        int eventsYesterday = await query
            .CountAsync(te => te.Timestamp >= yesterday.Date && te.Timestamp < yesterday.Date.AddDays(1), ct);

        double percentageChange = eventsYesterday > 0
            ? (double)(eventsLast24Hours - eventsYesterday) / eventsYesterday * 100
            : 0;

        string topCategory = await query
            .Where(te => te.Timestamp >= last24Hours)
            .GroupBy(te => te.Category)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefaultAsync(ct) ?? "Unknown";

        string topSourceCountry = await query
            .Where(te => te.Timestamp >= last24Hours && te.SourceCountry != null)
            .GroupBy(te => te.SourceCountry!.Name)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefaultAsync(ct) ?? "Unknown";

        List<RecentHighRiskEvent> recentHighRiskEvents = await query
            .Where(te => te.Timestamp >= lastHour)
            .Include(te => te.SourceCountry)
            .Include(te => te.MalwareFamily)
            .OrderByDescending(te => te.Timestamp)
            .Take(5)
            .Select(te => new RecentHighRiskEvent
            {
                EventId = te.Id,
                Timestamp = te.Timestamp,
                SourceAddress = te.SourceAddress,
                Category = te.Category,
                MalwareFamilyName = te.MalwareFamily != null ? te.MalwareFamily.Name : "Unknown",
                RiskScore = 7.5, // This would be calculated based on your risk scoring algorithm
                CountryName = te.SourceCountry != null ? te.SourceCountry.Name : "Unknown"
            })
            .ToListAsync(ct);

        return new ThreatEventDashboardMetrics
        {
            EventsLast24Hours = eventsLast24Hours,
            EventsLastHour = eventsLastHour,
            PercentageChangeFromYesterday = percentageChange,
            ActiveThreatsCurrently = eventsLastHour, // This could be refined based on your definition of "active"
            CriticalAlertsCount = recentHighRiskEvents.Count(e => e.RiskScore >= 8.0),
            TopThreatCategory = topCategory,
            TopSourceCountry = topSourceCountry,
            RecentHighRiskEvents = recentHighRiskEvents
        };
    }

    #endregion

    #region Categorical Analytics

    public async Task<IEnumerable<CategoryAnalytics>> GetTopCategoriesAsync(
        DateTime startTime,
        DateTime endTime,
        int topCount = 10,
        Guid? tenantId = null,
        CancellationToken ct = default)
    {
        // Ensure UTC timestamps for PostgreSQL compatibility
        startTime = DateTime.SpecifyKind(startTime, DateTimeKind.Utc);
        endTime = DateTime.SpecifyKind(endTime, DateTimeKind.Utc);

        IQueryable<ThreatEvent> query = _context.ThreatEvents
            .Where(te => te.Timestamp >= startTime && te.Timestamp <= endTime);

        if (tenantId.HasValue)
        {
            query = query.Where(te => _context.TenantAsnRegistries
                .Any(tar => tar.AsnRegistryId == te.AsnRegistryId && tar.TenantId == tenantId.Value));
        }

        int totalEvents = await query.CountAsync(ct);

        var categoryStats = await query
            .GroupBy(te => te.Category)
            .Select(g => new
            {
                Category = g.Key,
                Count = g.Count(),
                FirstSeen = g.Min(te => te.Timestamp),
                LastSeen = g.Max(te => te.Timestamp)
            })
            .OrderByDescending(x => x.Count)
            .Take(topCount)
            .ToListAsync(ct);

        // Calculate trends (compare with previous period)
        TimeSpan periodLength = endTime - startTime;
        DateTime previousStartTime = startTime - periodLength;
        DateTime previousEndTime = startTime;

        Dictionary<string, int> previousCounts = await _context.ThreatEvents
            .Where(te => te.Timestamp >= previousStartTime && te.Timestamp <= previousEndTime)
            // TODO: Multi-tenancy filter - add tenant filtering when available
            .GroupBy(te => te.Category)
            .Select(g => new { Category = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Category, x => x.Count, ct);

        return categoryStats.Select(stat =>
        {
            int previousCount = previousCounts.GetValueOrDefault(stat.Category, 0);
            double percentageChange = previousCount > 0
                ? (double)(stat.Count - previousCount) / previousCount * 100
                : 100;

            return new CategoryAnalytics
            {
                Category = stat.Category,
                Count = stat.Count,
                Percentage = totalEvents > 0 ? (double)stat.Count / totalEvents * 100 : 0,
                PercentageChange = percentageChange,
                TrendDirection = percentageChange > 5 ? "increasing" :
                                percentageChange < -5 ? "decreasing" : "stable",
                FirstSeen = stat.FirstSeen,
                LastSeen = stat.LastSeen
            };
        });
    }

    public async Task<IEnumerable<MalwareFamilyAnalytics>> GetMalwareFamilyAnalyticsAsync(
        DateTime startTime,
        DateTime endTime,
        int topCount = 10,
        Guid? tenantId = null,
        CancellationToken ct = default)
    {
        // Ensure UTC timestamps for PostgreSQL compatibility
        startTime = DateTime.SpecifyKind(startTime, DateTimeKind.Utc);
        endTime = DateTime.SpecifyKind(endTime, DateTimeKind.Utc);

        IQueryable<ThreatEvent> query = _context.ThreatEvents
            .Include(te => te.MalwareFamily)
            .Include(te => te.SourceCountry)
            .Where(te => te.Timestamp >= startTime && te.Timestamp <= endTime && te.MalwareFamilyId.HasValue);

        if (tenantId.HasValue)
        {
            query = query.Where(te => _context.TenantAsnRegistries
                .Any(tar => tar.AsnRegistryId == te.AsnRegistryId && tar.TenantId == tenantId.Value));
        }

        int totalEvents = await query.CountAsync(ct);

        var malwareFamilyStats = await query
            .GroupBy(te => new { te.MalwareFamilyId, te.MalwareFamily!.Name })
            .Select(g => new
            {
                MalwareFamilyId = g.Key.MalwareFamilyId!.Value,
                FamilyName = g.Key.Name,
                Count = g.Count(),
                FirstSeen = g.Min(te => te.Timestamp),
                LastSeen = g.Max(te => te.Timestamp),
                Categories = g.Select(te => te.Category).Distinct().ToList(),
                SourceCountries = g.Where(te => te.SourceCountry != null)
                    .Select(te => te.SourceCountry!.Name).Distinct().Take(5).ToList()
            })
            .OrderByDescending(x => x.Count)
            .Take(topCount)
            .ToListAsync(ct);

        return malwareFamilyStats.Select(stat => new MalwareFamilyAnalytics
        {
            MalwareFamilyId = stat.MalwareFamilyId,
            FamilyName = stat.FamilyName,
            Count = stat.Count,
            Percentage = totalEvents > 0 ? (double)stat.Count / totalEvents * 100 : 0,
            RiskScore = CalculateMalwareFamilyRiskScore(stat.Count, stat.Categories.Count),
            AssociatedCategories = stat.Categories,
            TopSourceCountries = stat.SourceCountries,
            FirstSeen = stat.FirstSeen,
            LastSeen = stat.LastSeen
        });
    }

    public async Task<IEnumerable<GeographicalAnalytics>> GetGeographicalAnalyticsAsync(
        DateTime startTime,
        DateTime endTime,
        int topCount = 20,
        Guid? tenantId = null,
        CancellationToken ct = default)
    {
        // Ensure UTC timestamps for PostgreSQL compatibility
        startTime = DateTime.SpecifyKind(startTime, DateTimeKind.Utc);
        endTime = DateTime.SpecifyKind(endTime, DateTimeKind.Utc);

        IQueryable<ThreatEvent> query = _context.ThreatEvents
            .Include(te => te.SourceCountry)
            .Include(te => te.MalwareFamily)
            .Where(te => te.Timestamp >= startTime && te.Timestamp <= endTime && te.SourceCountryId.HasValue);

        if (tenantId.HasValue)
        {
            query = query.Where(te => _context.TenantAsnRegistries
                .Any(tar => tar.AsnRegistryId == te.AsnRegistryId && tar.TenantId == tenantId.Value));
        }

        int totalEvents = await query.CountAsync(ct);

        var geoStats = await query
            .GroupBy(te => new { te.SourceCountryId, te.SourceCountry!.Name, te.SourceCountry.Code })
            .Select(g => new
            {
                CountryId = g.Key.SourceCountryId!.Value,
                CountryName = g.Key.Name,
                CountryCode = g.Key.Code,
                Count = g.Count(),
                Categories = g.Select(te => te.Category).Distinct().Take(5).ToList(),
                MalwareFamilies = g.Where(te => te.MalwareFamily != null)
                    .Select(te => te.MalwareFamily!.Name).Distinct().Take(5).ToList()
            })
            .OrderByDescending(x => x.Count)
            .Take(topCount)
            .ToListAsync(ct);

        return geoStats.Select(stat => new GeographicalAnalytics
        {
            CountryId = stat.CountryId,
            CountryName = stat.CountryName,
            CountryCode = stat.CountryCode,
            Count = stat.Count,
            Percentage = totalEvents > 0 ? (double)stat.Count / totalEvents * 100 : 0,
            IsSource = true, // This method focuses on source countries
            TopCategories = stat.Categories,
            TopMalwareFamilies = stat.MalwareFamilies,
            AverageRiskScore = CalculateCountryRiskScore(stat.Count, stat.Categories.Count)
        });
    }

    public async Task<IEnumerable<AsnAnalytics>> GetAsnAnalyticsAsync(
        DateTime startTime,
        DateTime endTime,
        int topCount = 15,
        Guid? tenantId = null,
        CancellationToken ct = default)
    {
        // Ensure UTC timestamps for PostgreSQL compatibility
        startTime = DateTime.SpecifyKind(startTime, DateTimeKind.Utc);
        endTime = DateTime.SpecifyKind(endTime, DateTimeKind.Utc);

        IQueryable<ThreatEvent> query = _context.ThreatEvents
            .Include(te => te.AsnRegistry)
            .Where(te => te.Timestamp >= startTime && te.Timestamp <= endTime);

        if (tenantId.HasValue)
        {
            query = query.Where(te => _context.TenantAsnRegistries
                .Any(tar => tar.AsnRegistryId == te.AsnRegistryId && tar.TenantId == tenantId.Value));
        }

        int totalEvents = await query.CountAsync(ct);

        var asnStats = await query
            .GroupBy(te => new { te.AsnRegistryId, te.AsnRegistry.Number, te.AsnRegistry.Description })
            .Select(g => new
            {
                g.Key.AsnRegistryId,
                AsnNumber = g.Key.Number,
                OrganizationName = g.Key.Description,
                Count = g.Count(),
                Categories = g.Select(te => te.Category).Distinct().Take(5).ToList(),
                SourceIps = g.Select(te => te.SourceAddress).Distinct().Take(10).ToList()
            })
            .OrderByDescending(x => x.Count)
            .Take(topCount)
            .ToListAsync(ct);

        return asnStats.Select(stat => new AsnAnalytics
        {
            AsnRegistryId = stat.AsnRegistryId,
            AsnNumber = stat.AsnNumber,
            OrganizationName = stat.OrganizationName,
            Count = stat.Count,
            Percentage = totalEvents > 0 ? (double)stat.Count / totalEvents * 100 : 0,
            TopCategories = stat.Categories,
            TopSourceIps = stat.SourceIps,
            AverageRiskScore = CalculateAsnRiskScore(stat.Count, stat.Categories.Count)
        });
    }

    #endregion

    #region Network Analytics

    public async Task<IEnumerable<PortAnalytics>> GetPortAnalyticsAsync(
        DateTime startTime,
        DateTime endTime,
        bool isSourcePort = true,
        int topCount = 20,
        Guid? tenantId = null,
        CancellationToken ct = default)
    {
        // Ensure UTC timestamps for PostgreSQL compatibility
        startTime = DateTime.SpecifyKind(startTime, DateTimeKind.Utc);
        endTime = DateTime.SpecifyKind(endTime, DateTimeKind.Utc);

        IQueryable<ThreatEvent> query = _context.ThreatEvents
            .Where(te => te.Timestamp >= startTime && te.Timestamp <= endTime);

        if (tenantId.HasValue)
        {
            query = query.Where(te => _context.TenantAsnRegistries
                .Any(tar => tar.AsnRegistryId == te.AsnRegistryId && tar.TenantId == tenantId.Value));
        }

        string portField = isSourcePort ? "SourcePort" : "DestinationPort";
        var portStats = isSourcePort
            ? await query.Where(te => te.SourcePort.HasValue)
                .GroupBy(te => te.SourcePort!.Value)
                .Select(g => new
                {
                    Port = g.Key,
                    Count = g.Count(),
                    Categories = g.Select(te => te.Category).Distinct().Take(3).ToList()
                })
                .OrderByDescending(x => x.Count)
                .Take(topCount)
                .ToListAsync(ct)
            : await query.Where(te => te.DestinationPort.HasValue)
                .GroupBy(te => te.DestinationPort!.Value)
                .Select(g => new
                {
                    Port = g.Key,
                    Count = g.Count(),
                    Categories = g.Select(te => te.Category).Distinct().Take(3).ToList()
                })
                .OrderByDescending(x => x.Count)
                .Take(topCount)
                .ToListAsync(ct);

        int totalEvents = await query.CountAsync(ct);

        return portStats.Select(stat => new PortAnalytics
        {
            Port = stat.Port,
            PortType = isSourcePort ? "source" : "destination",
            Count = stat.Count,
            Percentage = totalEvents > 0 ? (double)stat.Count / totalEvents * 100 : 0,
            AssociatedServices = GetKnownServices(stat.Port),
            TopCategories = stat.Categories,
            RiskScore = CalculatePortRiskScore(stat.Port, stat.Count)
        });
    }

    public async Task<IEnumerable<ProtocolAnalytics>> GetProtocolAnalyticsAsync(
        DateTime startTime,
        DateTime endTime,
        Guid? tenantId = null,
        CancellationToken ct = default)
    {
        // Ensure UTC timestamps for PostgreSQL compatibility
        startTime = DateTime.SpecifyKind(startTime, DateTimeKind.Utc);
        endTime = DateTime.SpecifyKind(endTime, DateTimeKind.Utc);

        IQueryable<ThreatEvent> query = _context.ThreatEvents
            .Include(te => te.Protocol)
            .Where(te => te.Timestamp >= startTime && te.Timestamp <= endTime && te.ProtocolId.HasValue);

        if (tenantId.HasValue)
        {
            query = query.Where(te => _context.TenantAsnRegistries
                .Any(tar => tar.AsnRegistryId == te.AsnRegistryId && tar.TenantId == tenantId.Value));
        }

        int totalEvents = await query.CountAsync(ct);

        var protocolStats = await query
            .GroupBy(te => new { te.ProtocolId, te.Protocol!.Name })
            .Select(g => new
            {
                ProtocolId = g.Key.ProtocolId!.Value,
                ProtocolName = g.Key.Name,
                Count = g.Count(),
                Categories = g.Select(te => te.Category).Distinct().Take(5).ToList(),
                Ports = g.Where(te => te.SourcePort.HasValue || te.DestinationPort.HasValue)
                    .Select(te => te.SourcePort ?? te.DestinationPort!.Value)
                    .Distinct().Take(10).ToList()
            })
            .OrderByDescending(x => x.Count)
            .ToListAsync(ct);

        return protocolStats.Select(stat => new ProtocolAnalytics
        {
            ProtocolId = stat.ProtocolId,
            ProtocolName = stat.ProtocolName,
            Count = stat.Count,
            Percentage = totalEvents > 0 ? (double)stat.Count / totalEvents * 100 : 0,
            TopPorts = stat.Ports,
            TopCategories = stat.Categories
        });
    }

    public async Task<IEnumerable<IpReputationAnalytics>> GetIpReputationAnalyticsAsync(
        DateTime startTime,
        DateTime endTime,
        int topCount = 50,
        bool isSourceIp = true,
        Guid? tenantId = null,
        CancellationToken ct = default)
    {
        // Ensure UTC timestamps for PostgreSQL compatibility
        startTime = DateTime.SpecifyKind(startTime, DateTimeKind.Utc);
        endTime = DateTime.SpecifyKind(endTime, DateTimeKind.Utc);

        IQueryable<ThreatEvent> query = _context.ThreatEvents
            .Include(te => te.SourceCountry)
            .Include(te => te.AsnRegistry)
            .Where(te => te.Timestamp >= startTime && te.Timestamp <= endTime);

        if (tenantId.HasValue)
        {
            query = query.Where(te => _context.TenantAsnRegistries
                .Any(tar => tar.AsnRegistryId == te.AsnRegistryId && tar.TenantId == tenantId.Value));
        }

        var ipStats = isSourceIp
            ? await query.GroupBy(te => te.SourceAddress)
                .Select(g => new
                {
                    IpAddress = g.Key,
                    Count = g.Count(),
                    FirstSeen = g.Min(te => te.Timestamp),
                    LastSeen = g.Max(te => te.Timestamp),
                    Categories = g.Select(te => te.Category).Distinct().ToList(),
                    CountryName = g.Select(te => te.SourceCountry != null ? te.SourceCountry.Name : "Unknown").FirstOrDefault() ?? "Unknown",
                    AsnDescription = g.Select(te => te.AsnRegistry.Description).FirstOrDefault() ?? "Unknown"
                })
                .OrderByDescending(x => x.Count)
                .Take(topCount)
                .ToListAsync(ct)
            : await query.Where(te => te.DestinationAddress != null)
                .GroupBy(te => te.DestinationAddress!)
                .Select(g => new
                {
                    IpAddress = g.Key,
                    Count = g.Count(),
                    FirstSeen = g.Min(te => te.Timestamp),
                    LastSeen = g.Max(te => te.Timestamp),
                    Categories = g.Select(te => te.Category).Distinct().ToList(),
                    CountryName = "Unknown", // Destination country lookup would need additional logic
                    AsnDescription = "Unknown"
                })
                .OrderByDescending(x => x.Count)
                .Take(topCount)
                .ToListAsync(ct);

        return ipStats.Select(stat => new IpReputationAnalytics
        {
            IpAddress = stat.IpAddress,
            IpType = isSourceIp ? "source" : "destination",
            Count = stat.Count,
            RiskScore = CalculateIpRiskScore(stat.Count, stat.Categories.Count),
            CountryName = stat.CountryName ?? "Unknown",
            AsnOrganization = stat.AsnDescription ?? "Unknown",
            AssociatedCategories = stat.Categories,
            FirstSeen = stat.FirstSeen,
            LastSeen = stat.LastSeen
        });
    }

    #endregion

    #region Advanced Analytics

    public async Task<IEnumerable<CorrelationPattern>> GetCorrelationPatternsAsync(
        DateTime startTime,
        DateTime endTime,
        string correlationType,
        Guid? tenantId = null,
        CancellationToken ct = default)
    {
        // Ensure UTC timestamps for PostgreSQL compatibility
        startTime = DateTime.SpecifyKind(startTime, DateTimeKind.Utc);
        endTime = DateTime.SpecifyKind(endTime, DateTimeKind.Utc);

        IQueryable<ThreatEvent> query = _context.ThreatEvents
            .Include(te => te.MalwareFamily)
            .Include(te => te.SourceCountry)
            .Where(te => te.Timestamp >= startTime && te.Timestamp <= endTime);

        if (tenantId.HasValue)
        {
            query = query.Where(te => _context.TenantAsnRegistries
                .Any(tar => tar.AsnRegistryId == te.AsnRegistryId && tar.TenantId == tenantId.Value));
        }

        List<ThreatEvent> events = await query.ToListAsync(ct);

        return correlationType switch
        {
            "category_malware" => AnalyzeCategoryMalwareCorrelation(events),
            "geo_port" => AnalyzeGeoPortCorrelation(events),
            "asn_category" => AnalyzeAsnCategoryCorrelation(events),
            _ => new List<CorrelationPattern>()
        };
    }

    public async Task<IEnumerable<AnomalyDetectionResult>> GetAnomalyDetectionAsync(
        DateTime startTime,
        DateTime endTime,
        string anomalyType,
        double sensitivityThreshold = 2.0,
        Guid? tenantId = null,
        CancellationToken ct = default)
    {
        // Ensure UTC timestamps for PostgreSQL compatibility
        startTime = DateTime.SpecifyKind(startTime, DateTimeKind.Utc);
        endTime = DateTime.SpecifyKind(endTime, DateTimeKind.Utc);

        IQueryable<ThreatEvent> query = _context.ThreatEvents
            .Where(te => te.Timestamp >= startTime && te.Timestamp <= endTime);

        if (tenantId.HasValue)
        {
            query = query.Where(te => _context.TenantAsnRegistries
                .Any(tar => tar.AsnRegistryId == te.AsnRegistryId && tar.TenantId == tenantId.Value));
        }

        // Simplified anomaly detection - in production, you'd use statistical models
        return anomalyType switch
        {
            "volume" => await DetectVolumeAnomalies(query, sensitivityThreshold, ct),
            "geographic" => await DetectGeographicAnomalies(query, sensitivityThreshold, ct),
            "temporal" => await DetectTemporalAnomalies(query, sensitivityThreshold, ct),
            _ => new List<AnomalyDetectionResult>()
        };
    }

    public async Task<IEnumerable<AttributionAnalytics>> GetAttributionAnalyticsAsync(
        DateTime startTime,
        DateTime endTime,
        Guid? tenantId = null,
        CancellationToken ct = default)
    {
        // This would integrate with threat intelligence feeds and attribution databases
        // For now, returning a placeholder implementation
        await Task.CompletedTask;
        return new List<AttributionAnalytics>();
    }

    #endregion

    #region Batch Operations

    public async Task<ThreatEventBatchAnalyticsResult> ExecuteBatchAnalyticsAsync(
        IEnumerable<ThreatEventAnalyticsQuery> queries,
        Guid? tenantId = null,
        CancellationToken ct = default)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var results = new Dictionary<string, object>();
        int queriesExecuted = 0;

        try
        {
            foreach (ThreatEventAnalyticsQuery query in queries)
            {
                object result = await ExecuteSingleAnalyticsQuery(query, tenantId, ct);
                results[query.QueryId] = result;
                queriesExecuted++;
            }

            stopwatch.Stop();

            return new ThreatEventBatchAnalyticsResult
            {
                Results = results,
                ExecutionTime = stopwatch.Elapsed,
                Success = true,
                QueriesExecuted = queriesExecuted
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            return new ThreatEventBatchAnalyticsResult
            {
                Results = results,
                ExecutionTime = stopwatch.Elapsed,
                Success = false,
                ErrorMessage = ex.Message,
                QueriesExecuted = queriesExecuted
            };
        }
    }

    public async Task<ThreatLandscapeOverview> GetThreatLandscapeOverviewAsync(
        DateTime startTime,
        DateTime endTime,
        Guid? tenantId = null,
        CancellationToken ct = default)
    {
        // Execute analytics tasks in parallel
        Task<ThreatEventSummary> summaryTask = GetSummaryAnalyticsAsync(startTime, endTime, tenantId, ct);
        Task<IEnumerable<CategoryAnalytics>> categoriesTask = GetTopCategoriesAsync(startTime, endTime, 10, tenantId, ct);
        Task<IEnumerable<GeographicalAnalytics>> geoTask = GetGeographicalAnalyticsAsync(startTime, endTime, 10, tenantId, ct);
        Task<IEnumerable<MalwareFamilyAnalytics>> malwareTask = GetMalwareFamilyAnalyticsAsync(startTime, endTime, 10, tenantId, ct);
        Task<IEnumerable<AsnAnalytics>> asnTask = GetAsnAnalyticsAsync(startTime, endTime, 10, tenantId, ct);
        Task<IEnumerable<TimelineDataPoint>> timelineTask = GetTimelineAnalyticsAsync(startTime, endTime, "hour", tenantId, null, null, null, ct);
        Task<IEnumerable<AnomalyDetectionResult>> anomalyTask = GetAnomalyDetectionAsync(startTime, endTime, "volume", 2.0, tenantId, ct);

        await Task.WhenAll(summaryTask, categoriesTask, geoTask, malwareTask, asnTask, timelineTask, anomalyTask);

        return new ThreatLandscapeOverview
        {
            Summary = await summaryTask,
            TopCategories = await categoriesTask,
            TopSourceCountries = await geoTask,
            TopMalwareFamilies = await malwareTask,
            TopAsns = await asnTask,
            HourlyTimeline = await timelineTask,
            RecentAnomalies = await anomalyTask,
            GeneratedAt = DateTime.UtcNow,
            AnalysisPeriod = endTime - startTime
        };
    }

    #endregion

    #region Helper Methods

    private string GetIntervalString(string interval)
    {
        return interval.ToLower() switch
        {
            "hour" => "1 hour",
            "day" => "1 day",
            "week" => "1 week",
            "month" => "1 month",
            _ => "1 hour"
        };
    }

    private double CalculateMalwareFamilyRiskScore(int eventCount, int categoryCount)
    {
        // Simplified risk scoring - you would implement your organization's risk model
        double baseScore = Math.Min(eventCount / 100.0, 5.0);
        double diversityScore = Math.Min(categoryCount * 0.5, 3.0);
        return Math.Min(baseScore + diversityScore, 10.0);
    }

    private double CalculateCountryRiskScore(int eventCount, int categoryCount)
    {
        double baseScore = Math.Min(eventCount / 200.0, 6.0);
        double diversityScore = Math.Min(categoryCount * 0.3, 2.0);
        return Math.Min(baseScore + diversityScore, 10.0);
    }

    private double CalculateAsnRiskScore(int eventCount, int categoryCount)
    {
        double baseScore = Math.Min(eventCount / 150.0, 5.5);
        double diversityScore = Math.Min(categoryCount * 0.4, 2.5);
        return Math.Min(baseScore + diversityScore, 10.0);
    }

    private double CalculatePortRiskScore(int port, int eventCount)
    {
        int[] commonPorts = new[] { 80, 443, 22, 25, 53, 110, 143, 993, 995 };
        bool isCommonPort = commonPorts.Contains(port);
        double baseScore = isCommonPort ? 3.0 : 6.0;
        double volumeScore = Math.Min(eventCount / 50.0, 4.0);
        return Math.Min(baseScore + volumeScore, 10.0);
    }

    private double CalculateIpRiskScore(int eventCount, int categoryCount)
    {
        double baseScore = Math.Min(eventCount / 20.0, 7.0);
        double diversityScore = Math.Min(categoryCount * 0.6, 3.0);
        return Math.Min(baseScore + diversityScore, 10.0);
    }

    private List<string> GetKnownServices(int port)
    {
        return port switch
        {
            22 => new List<string> { "SSH" },
            25 => new List<string> { "SMTP" },
            53 => new List<string> { "DNS" },
            80 => new List<string> { "HTTP" },
            443 => new List<string> { "HTTPS" },
            993 => new List<string> { "IMAPS" },
            995 => new List<string> { "POP3S" },
            _ => new List<string> { "Unknown" }
        };
    }

    private IEnumerable<CorrelationPattern> AnalyzeCategoryMalwareCorrelation(List<ThreatEvent> events)
    {
        IEnumerable<CorrelationPattern> correlations = events
            .Where(e => e.MalwareFamily != null)
            .GroupBy(e => new { e.Category, MalwareFamily = e.MalwareFamily!.Name })
            .Where(g => g.Count() > 1)
            .Select(g => new CorrelationPattern
            {
                PatternType = "category_malware",
                PrimaryKey = g.Key.Category,
                SecondaryKey = g.Key.MalwareFamily,
                Co_OccurrenceCount = g.Count(),
                CorrelationStrength = CalculateCorrelationStrength(g.Count(), events.Count),
                ConditionalProbability = CalculateConditionalProbability(g.Count(), events.Count(e => e.Category == g.Key.Category)),
                Description = $"Category '{g.Key.Category}' frequently appears with malware family '{g.Key.MalwareFamily}'"
            })
            .OrderByDescending(p => p.CorrelationStrength)
            .Take(10);

        return correlations;
    }

    private IEnumerable<CorrelationPattern> AnalyzeGeoPortCorrelation(List<ThreatEvent> events)
    {
        IEnumerable<CorrelationPattern> correlations = events
            .Where(e => e.SourceCountry != null && e.SourcePort.HasValue)
            .GroupBy(e => new { Country = e.SourceCountry!.Name, Port = e.SourcePort!.Value })
            .Where(g => g.Count() > 2)
            .Select(g => new CorrelationPattern
            {
                PatternType = "geo_port",
                PrimaryKey = g.Key.Country,
                SecondaryKey = g.Key.Port.ToString(),
                Co_OccurrenceCount = g.Count(),
                CorrelationStrength = CalculateCorrelationStrength(g.Count(), events.Count),
                ConditionalProbability = CalculateConditionalProbability(g.Count(), events.Count(e => e.SourceCountry?.Name == g.Key.Country)),
                Description = $"Country '{g.Key.Country}' frequently uses source port '{g.Key.Port}'"
            })
            .OrderByDescending(p => p.CorrelationStrength)
            .Take(10);

        return correlations;
    }

    private IEnumerable<CorrelationPattern> AnalyzeAsnCategoryCorrelation(List<ThreatEvent> events)
    {
        IEnumerable<CorrelationPattern> correlations = events
            .GroupBy(e => new { Asn = e.AsnRegistry.Description, e.Category })
            .Where(g => g.Count() > 1)
            .Select(g => new CorrelationPattern
            {
                PatternType = "asn_category",
                PrimaryKey = g.Key.Asn,
                SecondaryKey = g.Key.Category,
                Co_OccurrenceCount = g.Count(),
                CorrelationStrength = CalculateCorrelationStrength(g.Count(), events.Count),
                ConditionalProbability = CalculateConditionalProbability(g.Count(), events.Count(e => e.AsnRegistry.Description == g.Key.Asn)),
                Description = $"ASN '{g.Key.Asn}' frequently associated with category '{g.Key.Category}'"
            })
            .OrderByDescending(p => p.CorrelationStrength)
            .Take(10);

        return correlations;
    }

    private double CalculateCorrelationStrength(int coOccurrence, int totalEvents)
    {
        return totalEvents > 0 ? (double)coOccurrence / totalEvents : 0;
    }

    private double CalculateConditionalProbability(int coOccurrence, int conditionEvents)
    {
        return conditionEvents > 0 ? (double)coOccurrence / conditionEvents : 0;
    }

    private async Task<IEnumerable<AnomalyDetectionResult>> DetectVolumeAnomalies(
        IQueryable<ThreatEvent> query,
        double sensitivityThreshold,
        CancellationToken ct)
    {
        // Simplified volume anomaly detection
        var hourlyEvents = await query
            .GroupBy(e => new DateTime(e.Timestamp.Year, e.Timestamp.Month, e.Timestamp.Day, e.Timestamp.Hour, 0, 0))
            .Select(g => new { Hour = g.Key, Count = g.Count() })
            .OrderBy(x => x.Hour)
            .ToListAsync(ct);

        if (hourlyEvents.Count < 3)
        {

            return new List<AnomalyDetectionResult>();
        }


        double average = hourlyEvents.Average(x => x.Count);
        double stdDev = Math.Sqrt(hourlyEvents.Average(x => Math.Pow(x.Count - average, 2)));

        IEnumerable<AnomalyDetectionResult> anomalies = hourlyEvents
            .Where(x => Math.Abs(x.Count - average) > sensitivityThreshold * stdDev)
            .Select(x => new AnomalyDetectionResult
            {
                Timestamp = x.Hour,
                AnomalyType = "volume",
                AnomalyScore = Math.Abs(x.Count - average) / stdDev,
                ExpectedValue = average,
                ActualValue = x.Count,
                DeviationScore = (x.Count - average) / stdDev,
                Description = x.Count > average
                    ? $"Unusually high threat volume: {x.Count} events (expected ~{average:F0})"
                    : $"Unusually low threat volume: {x.Count} events (expected ~{average:F0})"
            });

        return anomalies;
    }

    private async Task<IEnumerable<AnomalyDetectionResult>> DetectGeographicAnomalies(
        IQueryable<ThreatEvent> query,
        double sensitivityThreshold,
        CancellationToken ct)
    {
        // Simplified geographic anomaly detection
        await Task.CompletedTask;
        return new List<AnomalyDetectionResult>();
    }

    private async Task<IEnumerable<AnomalyDetectionResult>> DetectTemporalAnomalies(
        IQueryable<ThreatEvent> query,
        double sensitivityThreshold,
        CancellationToken ct)
    {
        // Simplified temporal anomaly detection
        await Task.CompletedTask;
        return new List<AnomalyDetectionResult>();
    }

    private async Task<object> ExecuteSingleAnalyticsQuery(
        ThreatEventAnalyticsQuery query,
        Guid? tenantId,
        CancellationToken ct)
    {
        // This would be expanded to handle different query types
        await Task.CompletedTask;
        return new { query.QueryId, Result = "Placeholder" };
    }

    #endregion
}
