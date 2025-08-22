using MeUi.Application.Models.ThreatTemporal;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Interfaces;

public interface ITenantThreatTemporalRepository
{
    Task<List<TimeSeriesPointDto>> Get24HourAttackPatternAsync(Guid tenantId, DateTime start, DateTime end, CancellationToken cancellationToken = default);
    Task<List<DayOfWeekDto>> GetWeeklyAttackDistributionAsync(Guid tenantId, DateTime start, DateTime end, CancellationToken cancellationToken = default);
    Task<List<HourDayHeatmapDto>> GetHourDayHeatmapAsync(Guid tenantId, DateTime start, DateTime end, CancellationToken cancellationToken = default);
    Task<List<PeakActivityDto>> GetPeakActivityHoursAsync(Guid tenantId, DateTime start, DateTime end, int limit = 50, CancellationToken cancellationToken = default);
    Task<List<TimePeriodSeriesDto>> GetTimeOfDayPatternsAsync(Guid tenantId, DateTime start, DateTime end, TimeSpan interval, CancellationToken cancellationToken = default);
    Task<List<TimeSeriesPointDto>> GetWeekdayWeekendTrendsAsync(Guid tenantId, DateTime start, DateTime end, CancellationToken cancellationToken = default);
    Task<List<CampaignDurationDto>> GetCampaignDurationAnalysisAsync(Guid tenantId, DateTime start, DateTime end, int limit = 30, CancellationToken cancellationToken = default);
    Task<List<MonthlyGrowthDto>> GetMonthlyGrowthAnalysisAsync(Guid tenantId, int limit = 40, CancellationToken cancellationToken = default);
}