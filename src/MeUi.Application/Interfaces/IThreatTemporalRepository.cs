using MeUi.Application.Models.ThreatTemporal;

namespace MeUi.Application.Interfaces;

public interface IThreatTemporalRepository
{
    Task<List<TimeSeriesPointDto>> Get24HourAttackPatternAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default);
    Task<List<DayOfWeekDto>> GetWeeklyAttackDistributionAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default);
    Task<List<HourDayHeatmapDto>> GetHourDayHeatmapAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default);
    Task<List<PeakActivityDto>> GetPeakActivityByCategoryAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default);
    Task<List<TimePeriodSeriesDto>> GetAttackPatternsByTimeOfDayAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default);
    Task<List<TimeSeriesPointDto>> GetWeekdayWeekendTrendsAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default);
    Task<List<CampaignDurationDto>> GetAttackCampaignDurationAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default);
    Task<List<MonthlyGrowthDto>> GetMonthlyGrowthRateAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default);
}
