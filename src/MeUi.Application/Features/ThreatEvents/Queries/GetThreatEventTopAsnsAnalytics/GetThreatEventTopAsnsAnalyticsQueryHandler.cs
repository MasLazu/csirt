using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.Analytics;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventTopAsnsAnalytics;

public class GetThreatEventTopAsnsAnalyticsQueryHandler : IRequestHandler<GetThreatEventTopAsnsAnalyticsQuery, ThreatEventAsnTopAnalyticsDto>
{
    private readonly IThreatEventAnalyticsRepository _analyticsRepository;

    public GetThreatEventTopAsnsAnalyticsQueryHandler(IThreatEventAnalyticsRepository analyticsRepository)
    {
        _analyticsRepository = analyticsRepository;
    }

    public async Task<ThreatEventAsnTopAnalyticsDto> Handle(GetThreatEventTopAsnsAnalyticsQuery request, CancellationToken ct)
    {
        DateTime end = request.EndTime ?? DateTime.UtcNow;
        DateTime start = request.StartTime ?? end.AddDays(-30);
        if (start.Kind != DateTimeKind.Utc) start = start.ToUniversalTime();
        if (end.Kind != DateTimeKind.Utc) end = end.ToUniversalTime();

        var asns = (await _analyticsRepository.GetAsnAnalyticsAsync(start, end, request.TopLimit, null, ct)).ToList();
        int totalEvents = asns.Sum(a => a.Count);

        return new ThreatEventAsnTopAnalyticsDto
        {
            Asns = asns.Select(a => new AsnDistributionItemDto
            {
                AsnRegistryId = a.AsnRegistryId,
                AsnNumber = a.AsnNumber,
                OrganizationName = a.OrganizationName,
                Count = a.Count,
                Percentage = a.Percentage,
                TopCategories = a.TopCategories,
                TopSourceIps = a.TopSourceIps.Select(ip => ip.ToString()).ToList(),
                AverageRiskScore = a.AverageRiskScore
            }).ToList(),
            TotalEvents = totalEvents,
            TimeRange = new TimeRangeDto { Start = start, End = end },
            GeneratedAt = DateTime.UtcNow
        };
    }
}
