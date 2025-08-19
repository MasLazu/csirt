using MeUi.Application.Models.ThreatGeographic;

namespace MeUi.Application.Interfaces;

public interface IThreatGeographicRepository
{
    Task<List<CountryAttackTrendPointDto>> GetCountryAttackTrendsAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default);
    Task<List<CountryAttackRankingDto>> GetCountryRankingsAsync(DateTime start, DateTime end, int limit = 20, CancellationToken cancellationToken = default);
    Task<List<CountryAsnCorrelationDto>> GetCountryAsnCorrelationAsync(DateTime start, DateTime end, int limit = 25, CancellationToken cancellationToken = default);
    Task<List<RegionalTimeBucketDto>> GetRegionalTimeActivityAsync(DateTime start, DateTime end, int limit = 40, CancellationToken cancellationToken = default);
    Task<List<CrossBorderFlowDto>> GetCrossBorderFlowsAsync(DateTime start, DateTime end, int limit = 30, CancellationToken cancellationToken = default);
    Task<List<CategoryCountryTrendPointDto>> GetCategoryCountryTrendAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default);
}
