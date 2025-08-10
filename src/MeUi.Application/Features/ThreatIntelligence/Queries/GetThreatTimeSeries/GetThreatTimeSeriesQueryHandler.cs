using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Features.ThreatIntelligence.Models;
using MeUi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MeUi.Application.Exceptions;

namespace MeUi.Application.Features.ThreatIntelligence.Queries.GetThreatTimeSeries;

public class GetThreatTimeSeriesQueryHandler : IRequestHandler<GetThreatTimeSeriesNewQuery, ThreatTimeSeriesDto>
{
    private readonly IThreatEventRepository _repository;
    private readonly ITenantContext _tenantContext;

    public GetThreatTimeSeriesQueryHandler(
        IThreatEventRepository repository,
        ITenantContext tenantContext)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
    }

    public async Task<ThreatTimeSeriesDto> Handle(GetThreatTimeSeriesNewQuery request, CancellationToken ct)
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

            // Get time-series data based on interval and convert to unified DTO
            IEnumerable<TimeSeriesDataPointDto> timeSeries = request.Interval.ToLowerInvariant() switch
            {
                "hour" => (await _repository.GetHourlyThreatCountsAsync(startDateUtc, endDateUtc, ct))
                    .Select(h => new TimeSeriesDataPointDto
                    {
                        Timestamp = h.Hour,
                        ThreatCount = h.ThreatCount,
                        Interval = "hour"
                    }),
                "day" => (await _repository.GetDailyThreatCountsAsync(startDateUtc, endDateUtc, ct))
                    .Select(d => new TimeSeriesDataPointDto
                    {
                        Timestamp = d.Day,
                        ThreatCount = d.ThreatCount,
                        Interval = "day"
                    }),
                _ => (await _repository.GetHourlyThreatCountsAsync(startDateUtc, endDateUtc, ct))
                    .Select(h => new TimeSeriesDataPointDto
                    {
                        Timestamp = h.Hour,
                        ThreatCount = h.ThreatCount,
                        Interval = "hour"
                    })
            };

            return new ThreatTimeSeriesDto
            {
                TimeSeries = timeSeries,
                StartDate = startDateUtc,
                EndDate = endDateUtc,
                Interval = request.Interval,
                TotalDataPoints = timeSeries.Count()
            };
        }
        catch (TimeoutException ex)
        {
            throw new InvalidOperationException($"Time series query timeout: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error retrieving time series data: {ex.Message}", ex);
        }
    }
}