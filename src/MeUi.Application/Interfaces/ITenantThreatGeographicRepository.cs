using MeUi.Application.Models.ThreatGeographic;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Interfaces;

public interface ITenantThreatGeographicRepository
{
    Task<List<GeographicTrendDto>> GetGeographicTrendsAsync(Guid tenantId, DateTime start, DateTime end, TimeSpan interval, CancellationToken cancellationToken = default);
    Task<List<CountryRankingDto>> GetCountryRankingsAsync(Guid tenantId, DateTime start, DateTime end, int limit = 20, CancellationToken cancellationToken = default);
    Task<List<CountryAsnCorrelationDto>> GetCountryAsnCorrelationAsync(Guid tenantId, DateTime start, DateTime end, int limit = 25, CancellationToken cancellationToken = default);
    Task<List<RegionalTimeZoneActivityDto>> GetRegionalTimeZoneActivityAsync(Guid tenantId, DateTime start, DateTime end, int limit = 40, CancellationToken cancellationToken = default);
    Task<List<CrossBorderAttackFlowDto>> GetCrossBorderAttackFlowsAsync(Guid tenantId, DateTime start, DateTime end, int limit = 30, CancellationToken cancellationToken = default);
    Task<List<CountryCategoryTimelineDto>> GetCountryCategoryTimelineAsync(Guid tenantId, DateTime start, DateTime end, TimeSpan interval, int topCountries = 5, CancellationToken cancellationToken = default);
}