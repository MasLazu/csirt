using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.Analytics;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventGeoHeatmapAnalytics;

public class GetThreatEventGeoHeatmapAnalyticsQueryHandler : IRequestHandler<GetThreatEventGeoHeatmapAnalyticsQuery, ThreatEventGeoAnalyticsDto>
{
    private readonly IThreatEventAnalyticsRepository _analyticsRepository;

    public GetThreatEventGeoHeatmapAnalyticsQueryHandler(IThreatEventAnalyticsRepository analyticsRepository)
    {
        _analyticsRepository = analyticsRepository;
    }

    public async Task<ThreatEventGeoAnalyticsDto> Handle(GetThreatEventGeoHeatmapAnalyticsQuery request, CancellationToken ct)
    {
        DateTime end = request.EndTime ?? DateTime.UtcNow;
        DateTime start = request.StartTime ?? end.AddDays(-30);
        if (start.Kind != DateTimeKind.Utc) start = start.ToUniversalTime();
        if (end.Kind != DateTimeKind.Utc) end = end.ToUniversalTime();

        int top = request.TopCountriesLimit;
        var geo = (await _analyticsRepository.GetGeographicalAnalyticsAsync(start, end, top, null, ct)).ToList();

        var dto = new ThreatEventGeoAnalyticsDto
        {
            SourceCountries = geo.Select(g => new CountryThreatStatsDto
            {
                CountryId = g.CountryId,
                CountryName = g.CountryName,
                CountryCode = g.CountryCode,
                EventCount = g.Count,
                Percentage = g.Percentage,
                TopCategories = g.TopCategories,
                TopMalwareFamilies = g.TopMalwareFamilies
            }).ToList(),
            DestinationCountries = new(), // pending destination geo data
            ThreatFlows = new(), // requires destination link data model
            TotalUniqueCountries = geo.Count,
            TimeRange = new TimeRangeDto { Start = start, End = end }
        };
        return dto;
    }
}
