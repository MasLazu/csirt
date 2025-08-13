using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.Analytics;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetTenantThreatEventProtocolDistributionAnalytics;

public class GetTenantThreatEventProtocolDistributionAnalyticsQueryHandler : IRequestHandler<GetTenantThreatEventProtocolDistributionAnalyticsQuery, ThreatEventProtocolDistributionAnalyticsDto>
{
    private readonly IThreatEventAnalyticsRepository _analyticsRepository;

    public GetTenantThreatEventProtocolDistributionAnalyticsQueryHandler(IThreatEventAnalyticsRepository analyticsRepository)
    {
        _analyticsRepository = analyticsRepository;
    }

    public async Task<ThreatEventProtocolDistributionAnalyticsDto> Handle(GetTenantThreatEventProtocolDistributionAnalyticsQuery request, CancellationToken ct)
    {
        DateTime end = request.EndTime ?? DateTime.UtcNow;
        DateTime start = request.StartTime ?? end.AddDays(-30);
        if (start.Kind != DateTimeKind.Utc) start = start.ToUniversalTime();
        if (end.Kind != DateTimeKind.Utc) end = end.ToUniversalTime();

        var protocols = (await _analyticsRepository.GetProtocolAnalyticsAsync(start, end, request.TenantId, ct)).ToList();
        int totalEvents = protocols.Sum(p => p.Count);

        return new ThreatEventProtocolDistributionAnalyticsDto
        {
            Protocols = protocols.Select(p => new ProtocolDistributionItemDto
            {
                ProtocolId = p.ProtocolId,
                ProtocolName = p.ProtocolName,
                Count = p.Count,
                Percentage = p.Percentage,
                TopPorts = p.TopPorts,
                TopCategories = p.TopCategories
            }).ToList(),
            TotalEvents = totalEvents,
            GeneratedAt = DateTime.UtcNow,
            TimeRange = new TimeRangeDto { Start = start, End = end }
        };
    }
}
