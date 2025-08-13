using System.Linq.Expressions;
using MeUi.Domain.Entities;

namespace MeUi.Application.Interfaces;

/// <summary>
/// Specialized repository interface for analytics operations
/// Provides optimized methods for aggregations, time-series queries, and metrics calculation
/// </summary>
public interface IAnalyticsRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Execute time-bucketed aggregation queries (optimized for TimescaleDB)
    /// </summary>
    Task<IEnumerable<TTimelineResult>> GetTimelineDataAsync<TTimelineResult>(
        Expression<Func<T, bool>> predicate,
        string timeInterval, // "hour", "day", "week", "month"
        DateTime startTime,
        DateTime endTime,
        Expression<Func<IGrouping<DateTime, T>, TTimelineResult>> aggregationSelector,
        CancellationToken ct = default);

    /// <summary>
    /// Get grouped aggregations with counting and ranking
    /// </summary>
    Task<IEnumerable<TGroupResult>> GetGroupedAggregationAsync<TKey, TGroupResult>(
        Expression<Func<T, bool>> predicate,
        Expression<Func<T, TKey>> groupSelector,
        Expression<Func<IGrouping<TKey, T>, TGroupResult>> aggregationSelector,
        int? topCount = null,
        CancellationToken ct = default);

    /// <summary>
    /// Execute custom SQL queries for complex analytics (TimescaleDB specific)
    /// </summary>
    Task<IEnumerable<TResult>> ExecuteAnalyticsQueryAsync<TResult>(
        string sqlQuery,
        object? parameters = null,
        CancellationToken ct = default);

    /// <summary>
    /// Get top N items with counts and percentages
    /// </summary>
    Task<IEnumerable<TopItemResult<TKey>>> GetTopItemsAsync<TKey>(
        Expression<Func<T, bool>> predicate,
        Expression<Func<T, TKey>> keySelector,
        int topCount,
        CancellationToken ct = default);

    /// <summary>
    /// Calculate distribution percentages for categorical data
    /// </summary>
    Task<IEnumerable<DistributionResult<TKey>>> GetDistributionAsync<TKey>(
        Expression<Func<T, bool>> predicate,
        Expression<Func<T, TKey>> keySelector,
        CancellationToken ct = default);

    /// <summary>
    /// Get time-based trend comparison between two periods
    /// </summary>
    Task<TrendComparisonResult> GetTrendComparisonAsync(
        Expression<Func<T, bool>> currentPeriodPredicate,
        Expression<Func<T, bool>> comparisonPeriodPredicate,
        CancellationToken ct = default);

    /// <summary>
    /// Execute multi-dimensional aggregations (e.g., country x malware family)
    /// </summary>
    Task<IEnumerable<TResult>> GetMultiDimensionalAggregationAsync<TKey1, TKey2, TResult>(
        Expression<Func<T, bool>> predicate,
        Expression<Func<T, TKey1>> firstKeySelector,
        Expression<Func<T, TKey2>> secondKeySelector,
        Expression<Func<IGrouping<ValueTuple<TKey1, TKey2>, T>, TResult>> aggregationSelector,
        CancellationToken ct = default);

    /// <summary>
    /// Get statistical metrics (min, max, avg, std dev) for numeric fields
    /// </summary>
    Task<StatisticalMetrics> GetStatisticalMetricsAsync<TValue>(
        Expression<Func<T, bool>> predicate,
        Expression<Func<T, TValue>> valueSelector,
        CancellationToken ct = default) where TValue : struct, IComparable<TValue>;

    /// <summary>
    /// Get distinct value counts for cardinality analysis
    /// </summary>
    Task<CardinalityResult<TKey>> GetCardinalityAsync<TKey>(
        Expression<Func<T, bool>> predicate,
        Expression<Func<T, TKey>> keySelector,
        CancellationToken ct = default);

    /// <summary>
    /// Execute batch analytics queries for dashboard data
    /// </summary>
    Task<BatchAnalyticsResult> ExecuteBatchAnalyticsAsync(
        IEnumerable<AnalyticsQuery> queries,
        CancellationToken ct = default);
}

/// <summary>
/// Result model for top items with ranking information
/// </summary>
public class TopItemResult<TKey>
{
    public TKey Key { get; set; } = default!;
    public string DisplayName { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
    public int Rank { get; set; }
}

/// <summary>
/// Result model for distribution analysis
/// </summary>
public class DistributionResult<TKey>
{
    public TKey Key { get; set; } = default!;
    public string DisplayName { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
    public double CumulativePercentage { get; set; }
}

/// <summary>
/// Result model for trend comparison between periods
/// </summary>
public class TrendComparisonResult
{
    public int CurrentPeriodCount { get; set; }
    public int ComparisonPeriodCount { get; set; }
    public double PercentageChange { get; set; }
    public string TrendDirection { get; set; } = string.Empty; // "increasing", "decreasing", "stable"
    public double AbsoluteChange { get; set; }
}

/// <summary>
/// Statistical metrics for numeric analysis
/// </summary>
public class StatisticalMetrics
{
    public double Min { get; set; }
    public double Max { get; set; }
    public double Average { get; set; }
    public double StandardDeviation { get; set; }
    public double Median { get; set; }
    public int Count { get; set; }
    public double Sum { get; set; }
}

/// <summary>
/// Cardinality analysis result
/// </summary>
public class CardinalityResult<TKey>
{
    public int TotalRecords { get; set; }
    public int UniqueValues { get; set; }
    public double CardinalityRatio { get; set; } // unique/total
    public TKey MostFrequentValue { get; set; } = default!;
    public int MostFrequentCount { get; set; }
}

/// <summary>
/// Generic analytics query for batch processing
/// </summary>
public class AnalyticsQuery
{
    public string QueryId { get; set; } = string.Empty;
    public string SqlQuery { get; set; } = string.Empty;
    public object? Parameters { get; set; }
    public Type ResultType { get; set; } = typeof(object);
}

/// <summary>
/// Batch analytics execution result
/// </summary>
public class BatchAnalyticsResult
{
    public Dictionary<string, object> Results { get; set; } = new();
    public TimeSpan ExecutionTime { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}
