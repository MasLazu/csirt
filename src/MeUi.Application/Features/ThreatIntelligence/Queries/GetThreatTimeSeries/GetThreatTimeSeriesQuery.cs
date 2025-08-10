using MediatR;
using MeUi.Application.Features.ThreatIntelligence.Models;

namespace MeUi.Application.Features.ThreatIntelligence.Queries.GetThreatTimeSeries;

public record GetThreatTimeSeriesNewQuery : IRequest<ThreatTimeSeriesDto>
{
    /// <summary>
    /// Start date for time series calculation
    /// </summary>
    public DateTime StartDate { get; init; } = DateTime.UtcNow.AddDays(-7);

    /// <summary>
    /// End date for time series calculation
    /// </summary>
    public DateTime EndDate { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Time bucket interval (hour, day)
    /// </summary>
    public string Interval { get; init; } = "hour";
}

public class ThreatTimeSeriesDto
{
    public IEnumerable<TimeSeriesDataPointDto> TimeSeries { get; set; } = new List<TimeSeriesDataPointDto>();
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Interval { get; set; } = string.Empty;
    public int TotalDataPoints { get; set; }
}