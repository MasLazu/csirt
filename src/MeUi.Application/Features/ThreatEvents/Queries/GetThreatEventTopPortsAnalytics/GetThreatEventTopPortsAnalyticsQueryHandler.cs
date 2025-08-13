using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.Analytics;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventTopPortsAnalytics;

public class GetThreatEventTopPortsAnalyticsQueryHandler : IRequestHandler<GetThreatEventTopPortsAnalyticsQuery, ThreatEventPortTopAnalyticsDto>
{
    private readonly IThreatEventAnalyticsRepository _repository;

    public GetThreatEventTopPortsAnalyticsQueryHandler(IThreatEventAnalyticsRepository repository)
    {
        _repository = repository;
    }

    public async Task<ThreatEventPortTopAnalyticsDto> Handle(GetThreatEventTopPortsAnalyticsQuery request, CancellationToken cancellationToken)
    {
        var start = request.StartTime ?? DateTime.UtcNow.AddDays(-7); // default 7 days
        var end = request.EndTime ?? DateTime.UtcNow;

        var topSourcesTask = _repository.GetPortAnalyticsAsync(start, end, isSourcePort: true, topCount: request.Top, tenantId: null, ct: cancellationToken);
        Task<IEnumerable<PortAnalytics>>? topDestinationsTask = null;
        if (request.IncludeDestination)
        {
            topDestinationsTask = _repository.GetPortAnalyticsAsync(start, end, isSourcePort: false, topCount: request.Top, tenantId: null, ct: cancellationToken);
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
