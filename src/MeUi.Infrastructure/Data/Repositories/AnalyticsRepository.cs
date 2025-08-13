using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;
using MeUi.Infrastructure.Data;
using System.Diagnostics;

namespace MeUi.Infrastructure.Data.Repositories;

/// <summary>
/// Analytics repository implementation optimized for TimescaleDB time-series operations
/// </summary>
public class AnalyticsRepository<T> : IAnalyticsRepository<T> where T : BaseEntity
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<T> _dbSet;

    public AnalyticsRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<IEnumerable<TTimelineResult>> GetTimelineDataAsync<TTimelineResult>(
        Expression<Func<T, bool>> predicate,
        string timeInterval,
        DateTime startTime,
        DateTime endTime,
        Expression<Func<IGrouping<DateTime, T>, TTimelineResult>> aggregationSelector,
        CancellationToken ct = default)
    {
        IQueryable<T> query = _dbSet.Where(predicate);

        // Group by time intervals based on the specified interval
        IQueryable<IGrouping<DateTime, T>> timelineGroups = timeInterval.ToLower() switch
        {
            "hour" => query.GroupBy(GetHourBucket),
            "day" => query.GroupBy(GetDayBucket),
            "week" => query.GroupBy(GetWeekBucket),
            "month" => query.GroupBy(GetMonthBucket),
            _ => query.GroupBy(GetDayBucket) // Default to day
        };

        return await timelineGroups
            .Select(aggregationSelector)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<TGroupResult>> GetGroupedAggregationAsync<TKey, TGroupResult>(
        Expression<Func<T, bool>> predicate,
        Expression<Func<T, TKey>> groupSelector,
        Expression<Func<IGrouping<TKey, T>, TGroupResult>> aggregationSelector,
        int? topCount = null,
        CancellationToken ct = default)
    {
        IQueryable<TGroupResult> query = _dbSet
            .Where(predicate)
            .GroupBy(groupSelector)
            .Select(aggregationSelector);

        if (topCount.HasValue)
        {
            // For top results, we'll need to order by count
            // This assumes TGroupResult has a Count property - may need refinement
            query = query.Take(topCount.Value);
        }

        return await query.ToListAsync(ct);
    }

    public async Task<IEnumerable<TResult>> ExecuteAnalyticsQueryAsync<TResult>(
        string sqlQuery,
        object? parameters = null,
        CancellationToken ct = default)
    {
        // For TimescaleDB-specific queries using time_bucket() and other functions
        var results = new List<TResult>();

        try
        {
            using System.Data.Common.DbCommand command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = sqlQuery;

            // Add parameters if provided
            if (parameters != null)
            {
                AddParametersToCommand(command, parameters);
            }

            await _context.Database.OpenConnectionAsync(ct);
            using System.Data.Common.DbDataReader reader = await command.ExecuteReaderAsync(ct);

            while (await reader.ReadAsync(ct))
            {
                TResult? result = MapReaderToResult<TResult>(reader);
                if (result != null)
                {
                    results.Add(result);
                }
            }
        }
        finally
        {
            await _context.Database.CloseConnectionAsync();
        }

        return results;
    }

    public async Task<IEnumerable<TopItemResult<TKey>>> GetTopItemsAsync<TKey>(
        Expression<Func<T, bool>> predicate,
        Expression<Func<T, TKey>> keySelector,
        int topCount,
        CancellationToken ct = default)
    {
        int totalCount = await _dbSet.Where(predicate).CountAsync(ct);

        var topItems = await _dbSet
            .Where(predicate)
            .GroupBy(keySelector)
            .Select(g => new { g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(topCount)
            .ToListAsync(ct);

        return topItems.Select((item, index) => new TopItemResult<TKey>
        {
            Key = item.Key,
            DisplayName = item.Key?.ToString() ?? "Unknown",
            Count = item.Count,
            Percentage = totalCount > 0 ? Math.Round((double)item.Count / totalCount * 100, 2) : 0,
            Rank = index + 1
        });
    }

    public async Task<IEnumerable<DistributionResult<TKey>>> GetDistributionAsync<TKey>(
        Expression<Func<T, bool>> predicate,
        Expression<Func<T, TKey>> keySelector,
        CancellationToken ct = default)
    {
        int totalCount = await _dbSet.Where(predicate).CountAsync(ct);

        var distribution = await _dbSet
            .Where(predicate)
            .GroupBy(keySelector)
            .Select(g => new { g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToListAsync(ct);

        double cumulativePercentage = 0;
        return distribution.Select(item =>
        {
            double percentage = totalCount > 0 ? Math.Round((double)item.Count / totalCount * 100, 2) : 0;
            cumulativePercentage += percentage;

            return new DistributionResult<TKey>
            {
                Key = item.Key,
                DisplayName = item.Key?.ToString() ?? "Unknown",
                Count = item.Count,
                Percentage = percentage,
                CumulativePercentage = Math.Round(cumulativePercentage, 2)
            };
        });
    }

    public async Task<TrendComparisonResult> GetTrendComparisonAsync(
        Expression<Func<T, bool>> currentPeriodPredicate,
        Expression<Func<T, bool>> comparisonPeriodPredicate,
        CancellationToken ct = default)
    {
        int currentCount = await _dbSet.Where(currentPeriodPredicate).CountAsync(ct);
        int comparisonCount = await _dbSet.Where(comparisonPeriodPredicate).CountAsync(ct);

        int absoluteChange = currentCount - comparisonCount;
        double percentageChange = comparisonCount == 0
            ? (currentCount > 0 ? 100.0 : 0.0)
            : (double)absoluteChange / comparisonCount * 100.0;

        string trendDirection = percentageChange switch
        {
            > 5.0 => "increasing",
            < -5.0 => "decreasing",
            _ => "stable"
        };

        return new TrendComparisonResult
        {
            CurrentPeriodCount = currentCount,
            ComparisonPeriodCount = comparisonCount,
            PercentageChange = Math.Round(percentageChange, 2),
            TrendDirection = trendDirection,
            AbsoluteChange = absoluteChange
        };
    }

    public async Task<IEnumerable<TResult>> GetMultiDimensionalAggregationAsync<TKey1, TKey2, TResult>(
        Expression<Func<T, bool>> predicate,
        Expression<Func<T, TKey1>> firstKeySelector,
        Expression<Func<T, TKey2>> secondKeySelector,
        Expression<Func<IGrouping<ValueTuple<TKey1, TKey2>, T>, TResult>> aggregationSelector,
        CancellationToken ct = default)
    {
        return await _dbSet
            .Where(predicate)
            .GroupBy(x => new ValueTuple<TKey1, TKey2>(firstKeySelector.Compile()(x), secondKeySelector.Compile()(x)))
            .Select(aggregationSelector)
            .ToListAsync(ct);
    }

    public async Task<StatisticalMetrics> GetStatisticalMetricsAsync<TValue>(
        Expression<Func<T, bool>> predicate,
        Expression<Func<T, TValue>> valueSelector,
        CancellationToken ct = default) where TValue : struct, IComparable<TValue>
    {
        List<TValue> values = await _dbSet
            .Where(predicate)
            .Select(valueSelector)
            .ToListAsync(ct);

        if (!values.Any())
        {
            return new StatisticalMetrics();
        }

        var doubleValues = values.Select(v => Convert.ToDouble(v)).ToList();

        int count = doubleValues.Count;
        double sum = doubleValues.Sum();
        double average = doubleValues.Average();
        double min = doubleValues.Min();
        double max = doubleValues.Max();

        // Calculate standard deviation
        double variance = doubleValues.Sum(v => Math.Pow(v - average, 2)) / count;
        double standardDeviation = Math.Sqrt(variance);

        // Calculate median
        var sortedValues = doubleValues.OrderBy(v => v).ToList();
        double median = count % 2 == 0
            ? (sortedValues[count / 2 - 1] + sortedValues[count / 2]) / 2
            : sortedValues[count / 2];

        return new StatisticalMetrics
        {
            Count = count,
            Sum = sum,
            Average = Math.Round(average, 2),
            Min = min,
            Max = max,
            StandardDeviation = Math.Round(standardDeviation, 2),
            Median = Math.Round(median, 2)
        };
    }

    public async Task<CardinalityResult<TKey>> GetCardinalityAsync<TKey>(
        Expression<Func<T, bool>> predicate,
        Expression<Func<T, TKey>> keySelector,
        CancellationToken ct = default)
    {
        int totalRecords = await _dbSet.Where(predicate).CountAsync(ct);

        var distinctValues = await _dbSet
            .Where(predicate)
            .GroupBy(keySelector)
            .Select(g => new { g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToListAsync(ct);

        int uniqueValues = distinctValues.Count;
        double cardinalityRatio = totalRecords > 0 ? (double)uniqueValues / totalRecords : 0;

        var mostFrequent = distinctValues.FirstOrDefault();

        return new CardinalityResult<TKey>
        {
            TotalRecords = totalRecords,
            UniqueValues = uniqueValues,
            CardinalityRatio = Math.Round(cardinalityRatio, 4),
            MostFrequentValue = mostFrequent != null ? mostFrequent.Key : default!,
            MostFrequentCount = mostFrequent?.Count ?? 0
        };
    }

    public async Task<BatchAnalyticsResult> ExecuteBatchAnalyticsAsync(
        IEnumerable<AnalyticsQuery> queries,
        CancellationToken ct = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var results = new Dictionary<string, object>();
        bool success = true;
        string? errorMessage = null;

        try
        {
            foreach (AnalyticsQuery query in queries)
            {
                try
                {
                    // Execute individual query - this is a simplified implementation
                    // In production, you might want to use actual reflection or dynamic compilation
                    IEnumerable<object> queryResult = await ExecuteAnalyticsQueryAsync<object>(
                        query.SqlQuery,
                        query.Parameters,
                        ct);

                    results[query.QueryId] = queryResult;
                }
                catch (Exception ex)
                {
                    success = false;
                    errorMessage = $"Error executing query {query.QueryId}: {ex.Message}";
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            success = false;
            errorMessage = $"Batch execution error: {ex.Message}";
        }

        stopwatch.Stop();

        return new BatchAnalyticsResult
        {
            Results = results,
            ExecutionTime = stopwatch.Elapsed,
            Success = success,
            ErrorMessage = errorMessage
        };
    }

    // Helper methods for time bucketing
    private static Expression<Func<T, DateTime>> GetHourBucket =>
        entity => new DateTime(
            ((ThreatEvent)(object)entity).Timestamp.Year,
            ((ThreatEvent)(object)entity).Timestamp.Month,
            ((ThreatEvent)(object)entity).Timestamp.Day,
            ((ThreatEvent)(object)entity).Timestamp.Hour,
            0, 0);

    private static Expression<Func<T, DateTime>> GetDayBucket =>
        entity => new DateTime(
            ((ThreatEvent)(object)entity).Timestamp.Year,
            ((ThreatEvent)(object)entity).Timestamp.Month,
            ((ThreatEvent)(object)entity).Timestamp.Day);

    private static Expression<Func<T, DateTime>> GetWeekBucket =>
        entity => GetWeekStart(((ThreatEvent)(object)entity).Timestamp);

    private static Expression<Func<T, DateTime>> GetMonthBucket =>
        entity => new DateTime(
            ((ThreatEvent)(object)entity).Timestamp.Year,
            ((ThreatEvent)(object)entity).Timestamp.Month,
            1);

    private static DateTime GetWeekStart(DateTime date)
    {
        int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
        return date.AddDays(-diff).Date;
    }

    private static void AddParametersToCommand(System.Data.Common.DbCommand command, object parameters)
    {
        // Simplified parameter addition - in production you'd want more robust parameter handling
        System.Reflection.PropertyInfo[] properties = parameters.GetType().GetProperties();
        foreach (System.Reflection.PropertyInfo prop in properties)
        {
            System.Data.Common.DbParameter parameter = command.CreateParameter();
            parameter.ParameterName = $"@{prop.Name}";
            parameter.Value = prop.GetValue(parameters) ?? DBNull.Value;
            command.Parameters.Add(parameter);
        }
    }

    private static TResult? MapReaderToResult<TResult>(System.Data.Common.DbDataReader reader)
    {
        // Simplified mapping - in production you'd want more sophisticated object mapping
        // This could use libraries like Dapper for better performance and mapping
        try
        {
            if (typeof(TResult) == typeof(int))
            {
                return (TResult)(object)reader.GetInt32(0);
            }
            if (typeof(TResult) == typeof(string))
            {
                return (TResult)(object)reader.GetString(0);
            }
            if (typeof(TResult) == typeof(DateTime))
            {
                return (TResult)(object)reader.GetDateTime(0);
            }
            if (typeof(TResult) == typeof(double))
            {
                return (TResult)(object)reader.GetDouble(0);
            }

            // For complex types, you'd need more sophisticated mapping
            return default;
        }
        catch
        {
            return default;
        }
    }
}
