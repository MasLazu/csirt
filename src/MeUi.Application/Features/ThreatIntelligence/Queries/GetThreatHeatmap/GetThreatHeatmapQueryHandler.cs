using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Features.ThreatIntelligence.Models;
using MeUi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MeUi.Application.Exceptions;

namespace MeUi.Application.Features.ThreatIntelligence.Queries.GetThreatHeatmap;

public class GetThreatHeatmapQueryHandler : IRequestHandler<GetThreatHeatmapQuery, ThreatHeatmapDto>
{
    private readonly IThreatEventRepository _repository;
    private readonly ITenantContext _tenantContext;

    public GetThreatHeatmapQueryHandler(
        IThreatEventRepository repository,
        ITenantContext tenantContext)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
    }

    public async Task<ThreatHeatmapDto> Handle(GetThreatHeatmapQuery request, CancellationToken ct)
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

            var dataPoints = new List<HeatmapDataPointDto>();
            var totalThreats = 0L;

            switch (request.HeatmapType.ToLowerInvariant())
            {
                case "geographic":
                    dataPoints = await GenerateGeographicHeatmap(startDateUtc, endDateUtc, ct);
                    break;
                case "temporal":
                    dataPoints = await GenerateTemporalHeatmap(startDateUtc, endDateUtc, ct);
                    break;
                case "category":
                    dataPoints = await GenerateCategoryHeatmap(startDateUtc, endDateUtc, ct);
                    break;
                default:
                    dataPoints = await GenerateGeographicHeatmap(startDateUtc, endDateUtc, ct);
                    break;
            }

            totalThreats = dataPoints.Sum(dp => dp.Intensity);
            var maxIntensity = dataPoints.Any() ? dataPoints.Max(dp => dp.Intensity) : 0;
            var minIntensity = dataPoints.Any() ? dataPoints.Min(dp => dp.Intensity) : 0;

            var metadata = new HeatmapMetadataDto
            {
                TotalThreats = totalThreats,
                MaxIntensity = maxIntensity,
                MinIntensity = minIntensity,
                DataPointCount = dataPoints.Count,
                StartDate = startDateUtc,
                EndDate = endDateUtc,
                Resolution = $"{request.Resolution}x{request.Resolution}"
            };

            return new ThreatHeatmapDto
            {
                Type = request.HeatmapType,
                DataPoints = dataPoints,
                Metadata = metadata,
                GeneratedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error generating threat heatmap: {ex.Message}", ex);
        }
    }

    private async Task<List<HeatmapDataPointDto>> GenerateGeographicHeatmap(DateTime startDate, DateTime endDate, CancellationToken ct)
    {
        var countryThreats = await _repository.GetThreatCountsByCountryAsync(startDate, endDate, 50, ct);

        return countryThreats.Select(c => new HeatmapDataPointDto
        {
            X = GetCountryLongitude(c.CountryCode), // Longitude
            Y = GetCountryLatitude(c.CountryCode),  // Latitude
            Intensity = c.ThreatCount,
            Label = c.CountryName,
            Properties = new Dictionary<string, object>
            {
                ["countryCode"] = c.CountryCode,
                ["countryName"] = c.CountryName,
                ["threatCount"] = c.ThreatCount
            }
        }).ToList();
    }

    private async Task<List<HeatmapDataPointDto>> GenerateTemporalHeatmap(DateTime startDate, DateTime endDate, CancellationToken ct)
    {
        var hourlyData = await _repository.GetHourlyThreatCountsAsync(startDate, endDate, ct);

        return hourlyData.Select(h => new HeatmapDataPointDto
        {
            X = h.Hour.Hour, // Hour of day (0-23)
            Y = (h.Hour.Date - startDate.Date).Days, // Day offset
            Intensity = h.ThreatCount,
            Label = h.Hour.ToString("yyyy-MM-dd HH:00"),
            Properties = new Dictionary<string, object>
            {
                ["timestamp"] = h.Hour,
                ["hour"] = h.Hour.Hour,
                ["dayOffset"] = (h.Hour.Date - startDate.Date).Days,
                ["threatCount"] = h.ThreatCount
            }
        }).ToList();
    }

    private async Task<List<HeatmapDataPointDto>> GenerateCategoryHeatmap(DateTime startDate, DateTime endDate, CancellationToken ct)
    {
        var categoryData = await _repository.GetThreatCountsByCategoryAsync(startDate, endDate, ct);
        var categories = categoryData.ToList();

        var dataPoints = new List<HeatmapDataPointDto>();
        for (int i = 0; i < categories.Count; i++)
        {
            var category = categories[i];
            dataPoints.Add(new HeatmapDataPointDto
            {
                X = i % 5, // Grid position X (5 columns)
                Y = i / 5, // Grid position Y
                Intensity = category.ThreatCount,
                Label = category.Category,
                Properties = new Dictionary<string, object>
                {
                    ["category"] = category.Category,
                    ["threatCount"] = category.ThreatCount,
                    ["gridX"] = i % 5,
                    ["gridY"] = i / 5
                }
            });
        }

        return dataPoints;
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
            "AU" => -25.2744,
            "IT" => 41.8719,
            "ES" => 40.4637,
            "NL" => 52.1326,
            "KR" => 35.9078,
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
            "AU" => 133.7751,
            "IT" => 12.5674,
            "ES" => -3.7492,
            "NL" => 5.2913,
            "KR" => 127.7669,
            _ => 0.0
        };
    }
}