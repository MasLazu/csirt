using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Features.ThreatIntelligence.Models;
using MeUi.Application.Exceptions;
using MeUi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MeUi.Application.Features.ThreatIntelligence.Queries.GetThreatStatistics;

public class GetThreatStatisticsQueryHandler : IRequestHandler<GetThreatStatisticsQuery, ThreatStatisticsDto>
{
    private readonly IThreatEventRepository _repository;
    private readonly ITenantContext _tenantContext;

    public GetThreatStatisticsQueryHandler(
        IThreatEventRepository repository,
        ITenantContext tenantContext)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
    }

    public async Task<ThreatStatisticsDto> Handle(GetThreatStatisticsQuery request, CancellationToken ct)
    {
        // Validate tenant context - only authenticated users with valid tenant context can access threat intelligence
        if (!_tenantContext.IsSuperAdmin && !_tenantContext.TenantId.HasValue)
        {
            throw new TenantAccessDeniedException();
        }

        try
        {
            // Ensure DateTime parameters are in UTC for PostgreSQL compatibility
            var startDateUtc = request.StartDate.Kind == DateTimeKind.Utc
                ? request.StartDate
                : request.StartDate.ToUniversalTime();
            var endDateUtc = request.EndDate.Kind == DateTimeKind.Utc
                ? request.EndDate
                : request.EndDate.ToUniversalTime();

            // Get all threats in the time range
            var threats = await _repository.Query()
                .Where(t => t.Timestamp >= startDateUtc && t.Timestamp <= endDateUtc)
                .Include(t => t.AsnRegistry)
                .Include(t => t.SourceCountry)
                .ToListAsync(ct);

            var totalThreats = threats.Count;

            // Get time-series aggregations based on interval
            var hourlyThreatCounts = request.Interval.ToLowerInvariant() == "hour" || request.Interval.ToLowerInvariant() == "both"
                ? threats.GroupBy(t => new { t.Timestamp.Date, t.Timestamp.Hour })
                    .Select(g => new HourlyThreatCountDto
                    {
                        Hour = g.Key.Date.AddHours(g.Key.Hour),
                        ThreatCount = g.Count()
                    })
                    .OrderBy(h => h.Hour)
                    .ToList()
                : new List<HourlyThreatCountDto>();

            var dailyThreatCounts = request.Interval.ToLowerInvariant() == "day" || request.Interval.ToLowerInvariant() == "both"
                ? threats.GroupBy(t => t.Timestamp.Date)
                    .Select(g => new DailyThreatCountDto
                    {
                        Day = g.Key,
                        ThreatCount = g.Count()
                    })
                    .OrderBy(d => d.Day)
                    .ToList()
                : new List<DailyThreatCountDto>();

            // Get top threats by ASN
            var topAsnThreats = threats
                .GroupBy(t => new { t.AsnRegistry.Asn, t.AsnRegistry.Description })
                .Select(g => new AsnThreatCountDto
                {
                    Asn = g.Key.Asn,
                    AsnDescription = g.Key.Description,
                    ThreatCount = g.Count()
                })
                .OrderByDescending(a => a.ThreatCount)
                .Take(request.Limit)
                .ToList();

            // Get top threats by country
            var topCountryThreats = threats
                .Where(t => t.SourceCountry != null)
                .GroupBy(t => new { t.SourceCountry!.Code, t.SourceCountry.Name })
                .Select(g => new CountryThreatCountDto
                {
                    CountryCode = g.Key.Code,
                    CountryName = g.Key.Name,
                    ThreatCount = g.Count()
                })
                .OrderByDescending(c => c.ThreatCount)
                .Take(request.Limit)
                .ToList();

            // Get category breakdown
            var categoryBreakdown = threats
                .GroupBy(t => t.Category)
                .Select(g => new CategoryThreatCountDto
                {
                    Category = g.Key,
                    ThreatCount = g.Count()
                })
                .OrderByDescending(c => c.ThreatCount)
                .ToList();

            return new ThreatStatisticsDto
            {
                HourlyThreatCounts = hourlyThreatCounts,
                DailyThreatCounts = dailyThreatCounts,
                TopAsnThreats = topAsnThreats,
                TopCountryThreats = topCountryThreats,
                CategoryBreakdown = categoryBreakdown,
                StartDate = startDateUtc,
                EndDate = endDateUtc,
                TotalThreats = totalThreats
            };
        }
        catch (TimeoutException ex)
        {
            throw new InvalidOperationException($"Statistics query timeout: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error retrieving threat statistics: {ex.Message}", ex);
        }
    }
}