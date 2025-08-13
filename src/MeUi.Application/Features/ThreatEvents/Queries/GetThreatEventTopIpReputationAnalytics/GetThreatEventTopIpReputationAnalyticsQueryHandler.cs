using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.Analytics;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventTopIpReputationAnalytics;

public class GetThreatEventTopIpReputationAnalyticsQueryHandler : IRequestHandler<GetThreatEventTopIpReputationAnalyticsQuery, ThreatEventTopIpReputationAnalyticsDto>
{
    private readonly IThreatEventAnalyticsRepository _repository;

    public GetThreatEventTopIpReputationAnalyticsQueryHandler(IThreatEventAnalyticsRepository repository)
    {
        _repository = repository;
    }

    public async Task<ThreatEventTopIpReputationAnalyticsDto> Handle(GetThreatEventTopIpReputationAnalyticsQuery request, CancellationToken cancellationToken)
    {
        var start = request.StartTime ?? DateTime.UtcNow.AddDays(-7);
        var end = request.EndTime ?? DateTime.UtcNow;

        var topSourcesTask = _repository.GetIpReputationAnalyticsAsync(start, end, topCount: request.Top, isSourceIp: true, tenantId: null, ct: cancellationToken);
        Task<IEnumerable<IpReputationAnalytics>>? topDestinationsTask = null;
        if (request.IncludeDestination)
        {
            topDestinationsTask = _repository.GetIpReputationAnalyticsAsync(start, end, topCount: request.Top, isSourceIp: false, tenantId: null, ct: cancellationToken);
        }

        await Task.WhenAll(topSourcesTask, topDestinationsTask ?? Task.CompletedTask);

        return new ThreatEventTopIpReputationAnalyticsDto
        {
            TopSourceIps = topSourcesTask.Result.ToList(),
            TopDestinationIps = topDestinationsTask?.Result.ToList() ?? new List<IpReputationAnalytics>(),
            TimeRange = new TimeRangeDto { Start = start, End = end }
        };
    }
}
