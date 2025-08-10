using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Features.ThreatIntelligence.Models;
using MeUi.Application.Exceptions;
using MeUi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MeUi.Application.Features.ThreatIntelligence.Queries.GetThreatOverview;

public class GetThreatOverviewQueryHandler : IRequestHandler<GetThreatOverviewQuery, ThreatOverviewDto>
{
    private readonly IRepository<ThreatEvent> _repository;
    private readonly ITenantContext _tenantContext;

    public GetThreatOverviewQueryHandler(
        IRepository<ThreatEvent> repository,
        ITenantContext tenantContext)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
    }

    public async Task<ThreatOverviewDto> Handle(GetThreatOverviewQuery request, CancellationToken ct)
    {
        // Validate tenant context - only authenticated users with valid tenant context can access threat intelligence
        if (!_tenantContext.IsSuperAdmin && !_tenantContext.TenantId.HasValue)
        {
            throw new TenantAccessDeniedException();
        }

        try
        {
            var startDateUtc = request.StartDate.Kind == DateTimeKind.Utc
                ? request.StartDate
                : request.StartDate.ToUniversalTime();
            var endDateUtc = request.EndDate.Kind == DateTimeKind.Utc
                ? request.EndDate
                : request.EndDate.ToUniversalTime();

            // Get basic metrics using count queries
            var totalThreats = await _repository.CountAsync(t => t.Timestamp >= startDateUtc && t.Timestamp <= endDateUtc, ct);
            var todayThreats = await _repository.CountAsync(t => t.Timestamp >= DateTime.UtcNow.Date && t.Timestamp <= DateTime.UtcNow, ct);
            var weekThreats = await _repository.CountAsync(t => t.Timestamp >= DateTime.UtcNow.AddDays(-7) && t.Timestamp <= DateTime.UtcNow, ct);
            var monthThreats = await _repository.CountAsync(t => t.Timestamp >= DateTime.UtcNow.AddDays(-30) && t.Timestamp <= DateTime.UtcNow, ct);

            // Get daily trends using group by
            var threats = await _repository.Query()
                .Where(t => t.Timestamp >= startDateUtc && t.Timestamp <= endDateUtc)
                .Select(t => new { t.Timestamp, t.SourceCountry })
                .ToListAsync(ct);

            var dailyTrends = threats
                .GroupBy(t => t.Timestamp.Date)
                .Select(g => new ThreatTrendDto
                {
                    Date = g.Key,
                    ThreatCount = g.Count(),
                    Period = "day"
                })
                .OrderBy(t => t.Date);

            // Get top threats by country
            var topCountries = threats
                .Where(t => t.SourceCountry != null)
                .GroupBy(t => t.SourceCountry!.Name)
                .Select(g => new { CountryName = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .ToList();

            var topThreats = topCountries.Select(c => new TopThreatDto
            {
                Name = c.CountryName,
                Type = "country",
                Count = c.Count,
                Percentage = totalThreats > 0 ? (double)c.Count / totalThreats * 100 : 0,
                Severity = GetCountrySeverity(c.Count, totalThreats)
            });

            // Get geographic distribution
            var geoDistribution = topCountries.Select(c => new GeographicThreatDto
            {
                CountryCode = GetCountryCode(c.CountryName),
                CountryName = c.CountryName,
                ThreatCount = c.Count,
                Percentage = totalThreats > 0 ? (double)c.Count / totalThreats * 100 : 0,
                Latitude = GetCountryLatitude(GetCountryCode(c.CountryName)),
                Longitude = GetCountryLongitude(GetCountryCode(c.CountryName))
            });

            // Calculate growth rate (simplified)
            var previousPeriodThreats = await _repository.CountAsync(t => 
                t.Timestamp >= startDateUtc.AddDays(-(endDateUtc - startDateUtc).Days) && 
                t.Timestamp <= startDateUtc, ct);
            var growthRate = previousPeriodThreats > 0
                ? ((double)(totalThreats - previousPeriodThreats) / previousPeriodThreats) * 100
                : 0;

            // Get unique counts
            var uniqueSourceIps = await _repository.Query()
                .Where(t => t.Timestamp >= startDateUtc && t.Timestamp <= endDateUtc)
                .Select(t => t.SourceAddress.ToString())
                .Distinct()
                .CountAsync(ct);

            var uniqueAsns = await _repository.Query()
                .Where(t => t.Timestamp >= startDateUtc && t.Timestamp <= endDateUtc)
                .Include(t => t.AsnRegistry)
                .Select(t => t.AsnRegistry.Asn)
                .Distinct()
                .CountAsync(ct);

            var affectedCountries = await _repository.Query()
                .Where(t => t.Timestamp >= startDateUtc && t.Timestamp <= endDateUtc && t.SourceCountry != null)
                .Select(t => t.SourceCountry!.Code)
                .Distinct()
                .CountAsync(ct);

            var threatCategories = await _repository.Query()
                .Where(t => t.Timestamp >= startDateUtc && t.Timestamp <= endDateUtc)
                .Select(t => t.Category)
                .Distinct()
                .CountAsync(ct);

            var metrics = new ThreatMetricsDto
            {
                TotalThreats = (int)totalThreats,
                ThreatsToday = (int)todayThreats,
                ThreatsThisWeek = (int)weekThreats,
                ThreatsThisMonth = (int)monthThreats,
                ThreatGrowthRate = growthRate,
                UniqueSourceIps = uniqueSourceIps,
                UniqueAsns = uniqueAsns,
                AffectedCountries = affectedCountries,
                ThreatCategories = threatCategories
            };

            return new ThreatOverviewDto
            {
                Metrics = metrics,
                ThreatTrends = dailyTrends,
                TopThreats = topThreats,
                GeographicDistribution = geoDistribution,
                LastUpdated = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error retrieving threat overview: {ex.Message}", ex);
        }
    }

    private static string GetCountrySeverity(int threatCount, long totalThreats)
    {
        var percentage = totalThreats > 0 ? (double)threatCount / totalThreats * 100 : 0;
        return percentage switch
        {
            > 20 => "Critical",
            > 10 => "High",
            > 5 => "Medium",
            _ => "Low"
        };
    }

    private static string GetCountryCode(string countryName)
    {
        return countryName switch
        {
            "United States" => "US",
            "China" => "CN",
            "Russia" => "RU",
            "Germany" => "DE",
            "United Kingdom" => "GB",
            "France" => "FR",
            "Japan" => "JP",
            "India" => "IN",
            "Brazil" => "BR",
            "Canada" => "CA",
            _ => "XX"
        };
    }

    private static double GetCountryLatitude(string countryCode)
    {
        return countryCode switch
        {
            "US" => 39.8283,
            "CN" => 35.8617,
            "RU" => 61.5240,
            "DE" => 51.1657,
            "GB" => 55.3781,
            "FR" => 46.2276,
            "JP" => 36.2048,
            "IN" => 20.5937,
            "BR" => -14.2350,
            "CA" => 56.1304,
            _ => 0.0
        };
    }

    private static double GetCountryLongitude(string countryCode)
    {
        return countryCode switch
        {
            "US" => -98.5795,
            "CN" => 104.1954,
            "RU" => 105.3188,
            "DE" => 10.4515,
            "GB" => -3.4360,
            "FR" => 2.2137,
            "JP" => 138.2529,
            "IN" => 78.9629,
            "BR" => -51.9253,
            "CA" => -106.3468,
            _ => 0.0
        };
    }
}