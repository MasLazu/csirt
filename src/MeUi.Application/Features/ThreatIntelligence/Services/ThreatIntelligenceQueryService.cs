using System.Linq.Expressions;
using MeUi.Application.Features.ThreatIntelligence.Interfaces;
using MeUi.Application.Features.ThreatIntelligence.Models;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.ThreatIntelligence.Services;

public class ThreatIntelligenceQueryService : IThreatIntelligenceQueryService
{
    private readonly IRepository<MeUi.Domain.Entities.ThreatIntelligence> _repository;

    public ThreatIntelligenceQueryService(IRepository<MeUi.Domain.Entities.ThreatIntelligence> repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<IEnumerable<MeUi.Domain.Entities.ThreatIntelligence>> GetByFilterAsync(ThreatIntelligenceFilter filter, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(filter);

        var predicate = BuildFilterExpression(filter);
        return await _repository.FindAsync(predicate, ct);
    }

    public async Task<(IEnumerable<MeUi.Domain.Entities.ThreatIntelligence> Items, int TotalCount)> GetPaginatedAsync(ThreatIntelligenceFilter filter, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(filter);

        var predicate = BuildFilterExpression(filter);
        var orderBy = BuildOrderByExpression(filter.SortBy);

        return await _repository.GetPaginatedAsync(
            predicate: predicate,
            orderBy: orderBy,
            orderByDescending: filter.SortDescending,
            skip: filter.Skip,
            take: filter.Take,
            ct: ct);
    }

    /// <summary>
    /// Builds a LINQ expression from the filter parameters
    /// Handles multiple criteria combinations (AND conditions) and supports nested property filtering
    /// </summary>
    /// <param name="filter">Filter criteria</param>
    /// <returns>Expression for filtering threat intelligence records</returns>
    private Expression<Func<MeUi.Domain.Entities.ThreatIntelligence, bool>> BuildFilterExpression(ThreatIntelligenceFilter filter)
    {
        if (filter == null)
            throw new ArgumentNullException(nameof(filter));

        Expression<Func<MeUi.Domain.Entities.ThreatIntelligence, bool>> predicate = t => true;

        // Filter by ASN - exact match
        if (!string.IsNullOrWhiteSpace(filter.Asn))
        {
            predicate = CombineExpressions(predicate, t => t.Asn == filter.Asn);
        }

        // Filter by source address - exact match
        if (!string.IsNullOrWhiteSpace(filter.SourceAddress))
        {
            predicate = CombineExpressions(predicate, t => t.SourceAddress == filter.SourceAddress);
        }

        // Filter by destination address (nested property) - exact match with null safety
        if (!string.IsNullOrWhiteSpace(filter.DestinationAddress))
        {
            predicate = CombineExpressions(predicate, t =>
                t.OptionalInformation != null &&
                t.OptionalInformation.DestinationAddress == filter.DestinationAddress);
        }

        // Filter by source country - exact match
        if (!string.IsNullOrWhiteSpace(filter.SourceCountry))
        {
            predicate = CombineExpressions(predicate, t => t.SourceCountry == filter.SourceCountry);
        }

        // Filter by destination country (nested property) - exact match with null safety
        if (!string.IsNullOrWhiteSpace(filter.DestinationCountry))
        {
            predicate = CombineExpressions(predicate, t =>
                t.OptionalInformation != null &&
                t.OptionalInformation.DestinationCountry == filter.DestinationCountry);
        }

        // Filter by category - exact match
        if (!string.IsNullOrWhiteSpace(filter.Category))
        {
            predicate = CombineExpressions(predicate, t => t.Category == filter.Category);
        }

        // Filter by protocol (nested property) - exact match with null safety
        if (!string.IsNullOrWhiteSpace(filter.Protocol))
        {
            predicate = CombineExpressions(predicate, t =>
                t.OptionalInformation != null &&
                t.OptionalInformation.Protocol == filter.Protocol);
        }

        // Filter by source port (nested property) - exact match with null safety
        if (filter.SourcePort.HasValue)
        {
            var sourcePortString = filter.SourcePort.Value.ToString();
            predicate = CombineExpressions(predicate, t =>
                t.OptionalInformation != null &&
                t.OptionalInformation.SourcePort == sourcePortString);
        }

        // Filter by destination port (nested property) - exact match with null safety
        if (filter.DestinationPort.HasValue)
        {
            var destinationPortString = filter.DestinationPort.Value.ToString();
            predicate = CombineExpressions(predicate, t =>
                t.OptionalInformation != null &&
                t.OptionalInformation.DestinationPort == destinationPortString);
        }

        // Filter by malware family (nested property) - exact match with null safety
        if (!string.IsNullOrWhiteSpace(filter.MalwareFamily))
        {
            predicate = CombineExpressions(predicate, t =>
                t.OptionalInformation != null &&
                t.OptionalInformation.Family == filter.MalwareFamily);
        }

        // Date range filtering on Timestamp property
        // Validate date range before applying filters
        ValidateDateRange(filter.StartDate, filter.EndDate);

        // Filter by start date - records from this date onwards (inclusive)
        if (filter.StartDate.HasValue)
        {
            predicate = CombineExpressions(predicate, t => t.Timestamp >= filter.StartDate.Value);
        }

        // Filter by end date - records up to this date (inclusive)
        if (filter.EndDate.HasValue)
        {
            predicate = CombineExpressions(predicate, t => t.Timestamp <= filter.EndDate.Value);
        }

        return predicate;
    }

    /// <summary>
    /// Builds an order by expression based on the sort field
    /// </summary>
    /// <param name="sortBy">Field to sort by</param>
    /// <returns>Expression for ordering</returns>
    private Expression<Func<MeUi.Domain.Entities.ThreatIntelligence, object>>? BuildOrderByExpression(string? sortBy)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
        {
            // Default sort by timestamp
            return t => t.Timestamp;
        }

        return sortBy.ToLowerInvariant() switch
        {
            "timestamp" => t => t.Timestamp,
            "asn" => t => t.Asn,
            "sourceaddress" => t => t.SourceAddress,
            "sourcecountry" => t => t.SourceCountry,
            "category" => t => t.Category,
            "destinationaddress" => t => t.OptionalInformation.DestinationAddress ?? string.Empty,
            "destinationcountry" => t => t.OptionalInformation.DestinationCountry ?? string.Empty,
            "protocol" => t => t.OptionalInformation.Protocol ?? string.Empty,
            "sourceport" => t => t.OptionalInformation.SourcePort ?? string.Empty,
            "destinationport" => t => t.OptionalInformation.DestinationPort ?? string.Empty,
            "family" => t => t.OptionalInformation.Family ?? string.Empty,
            _ => t => t.Timestamp // Default fallback
        };
    }

    /// <summary>
    /// Combines two expressions using AND logic
    /// </summary>
    /// <param name="first">First expression</param>
    /// <param name="second">Second expression</param>
    /// <returns>Combined expression</returns>
    private Expression<Func<MeUi.Domain.Entities.ThreatIntelligence, bool>> CombineExpressions(
        Expression<Func<MeUi.Domain.Entities.ThreatIntelligence, bool>> first,
        Expression<Func<MeUi.Domain.Entities.ThreatIntelligence, bool>> second)
    {
        return CombineExpressions(first, second, ExpressionType.AndAlso);
    }

    /// <summary>
    /// Combines two expressions using the specified logic (AND/OR)
    /// </summary>
    /// <param name="first">First expression</param>
    /// <param name="second">Second expression</param>
    /// <param name="combineType">Type of combination (AndAlso or OrElse)</param>
    /// <returns>Combined expression</returns>
    private Expression<Func<MeUi.Domain.Entities.ThreatIntelligence, bool>> CombineExpressions(
        Expression<Func<MeUi.Domain.Entities.ThreatIntelligence, bool>> first,
        Expression<Func<MeUi.Domain.Entities.ThreatIntelligence, bool>> second,
        ExpressionType combineType)
    {
        var parameter = Expression.Parameter(typeof(MeUi.Domain.Entities.ThreatIntelligence), "t");

        var firstBody = ReplaceParameter(first.Body, first.Parameters[0], parameter);
        var secondBody = ReplaceParameter(second.Body, second.Parameters[0], parameter);

        var combined = combineType switch
        {
            ExpressionType.AndAlso => Expression.AndAlso(firstBody, secondBody),
            ExpressionType.OrElse => Expression.OrElse(firstBody, secondBody),
            _ => throw new ArgumentException($"Unsupported expression type: {combineType}", nameof(combineType))
        };

        return Expression.Lambda<Func<MeUi.Domain.Entities.ThreatIntelligence, bool>>(combined, parameter);
    }

    /// <summary>
    /// Creates an expression for checking if a nested property is not null and matches a value
    /// </summary>
    /// <param name="propertySelector">Property selector expression</param>
    /// <param name="value">Value to match</param>
    /// <returns>Expression with null safety check</returns>
    private Expression<Func<MeUi.Domain.Entities.ThreatIntelligence, bool>> CreateNestedPropertyExpression(
        Expression<Func<MeUi.Domain.Entities.ThreatIntelligence, string?>> propertySelector,
        string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value cannot be null or whitespace", nameof(value));

        return t => propertySelector.Compile()(t) == value;
    }

    /// <summary>
    /// Validates that date range filters are logically correct
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <exception cref="ArgumentException">Thrown when date range is invalid</exception>
    private static void ValidateDateRange(DateTime? startDate, DateTime? endDate)
    {
        if (startDate.HasValue && endDate.HasValue && startDate.Value > endDate.Value)
        {
            throw new ArgumentException("Start date cannot be greater than end date");
        }
    }

    /// <summary>
    /// Replaces parameter in expression
    /// </summary>
    /// <param name="expression">Expression to modify</param>
    /// <param name="oldParameter">Old parameter to replace</param>
    /// <param name="newParameter">New parameter to use</param>
    /// <returns>Modified expression</returns>
    private Expression ReplaceParameter(Expression expression, ParameterExpression oldParameter, ParameterExpression newParameter)
    {
        return new ParameterReplacer(oldParameter, newParameter).Visit(expression);
    }

    /// <summary>
    /// Expression visitor for replacing parameters
    /// </summary>
    private class ParameterReplacer : ExpressionVisitor
    {
        private readonly ParameterExpression _oldParameter;
        private readonly ParameterExpression _newParameter;

        public ParameterReplacer(ParameterExpression oldParameter, ParameterExpression newParameter)
        {
            _oldParameter = oldParameter;
            _newParameter = newParameter;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == _oldParameter ? _newParameter : base.VisitParameter(node);
        }
    }
}