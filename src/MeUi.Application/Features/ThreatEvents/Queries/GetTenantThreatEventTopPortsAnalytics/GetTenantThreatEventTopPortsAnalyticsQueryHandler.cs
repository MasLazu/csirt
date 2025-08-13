using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.Analytics;
using MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventTopPortsAnalytics;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetTenantThreatEventTopPortsAnalytics;

public class GetTenantThreatEventTopPortsAnalyticsQueryHandler : IRequestHandler<GetTenantThreatEventTopPortsAnalyticsQuery, ThreatEventPortTopAnalyticsDto>
{
    private readonly IThreatEventAnalyticsRepository _repository;

    public GetTenantThreatEventTopPortsAnalyticsQueryHandler(IThreatEventAnalyticsRepository repository)
    {
        _repository = repository;
    }

    public async Task<ThreatEventPortTopAnalyticsDto> Handle(GetTenantThreatEventTopPortsAnalyticsQuery request, CancellationToken cancellationToken)
    {
        var start = request.StartTime ?? DateTime.UtcNow.AddDays(-7);
        var end = request.EndTime ?? DateTime.UtcNow;

        var topSourcesTask = _repository.GetPortAnalyticsAsync(start, end, isSourcePort: true, topCount: request.Top, tenantId: request.TenantId, ct: cancellationToken);
        Task<IEnumerable<PortAnalytics>>? topDestinationsTask = null;
        if (request.IncludeDestination)
        {
            topDestinationsTask = _repository.GetPortAnalyticsAsync(start, end, isSourcePort: false, topCount: request.Top, tenantId: request.TenantId, ct: cancellationToken);
        }

        await Task.WhenAll(topSourcesTask, topDestinationsTask ?? Task.CompletedTask);

        return new ThreatEventPortTopAnalyticsDto
        {
            TopSourcePorts = topSourcesTask.Result.ToList(),
            TopDestinationPorts = topDestinationsTask?.Result.ToList() ?? new List<PortAnalytics>(),
            TimeRange = new TimeRangeDto { Start = start, End = end }
        };
    }
}
