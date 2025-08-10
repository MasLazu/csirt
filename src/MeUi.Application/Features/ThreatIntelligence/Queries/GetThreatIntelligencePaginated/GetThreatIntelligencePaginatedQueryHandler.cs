using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Features.ThreatIntelligence.Models;
using MeUi.Application.Exceptions;
using MeUi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MeUi.Application.Features.ThreatIntelligence.Queries.GetThreatIntelligencePaginated;

public class GetThreatIntelligencePaginatedQueryHandler : IRequestHandler<GetThreatIntelligencePaginatedQuery, PaginatedThreatIntelligenceDto>
{
    private readonly IRepository<ThreatEvent> _repository;
    private readonly ITenantContext _tenantContext;

    public GetThreatIntelligencePaginatedQueryHandler(
        IRepository<ThreatEvent> repository,
        ITenantContext tenantContext)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
    }

    public async Task<PaginatedThreatIntelligenceDto> Handle(GetThreatIntelligencePaginatedQuery request, CancellationToken ct)
    {
        // Validate tenant context - only authenticated users with valid tenant context can access threat intelligence
        if (!_tenantContext.IsSuperAdmin && !_tenantContext.TenantId.HasValue)
        {
            throw new TenantAccessDeniedException();
        }

        try
        {
            // Build filter predicate
            Expression<Func<ThreatEvent, bool>> predicate = t => true;
            
            if (request.StartDate.HasValue)
            {
                var startDate = request.StartDate.Value;
                predicate = CombineExpressions(predicate, t => t.Timestamp >= startDate);
            }
            
            if (request.EndDate.HasValue)
            {
                var endDate = request.EndDate.Value;
                predicate = CombineExpressions(predicate, t => t.Timestamp <= endDate);
            }
            
            if (!string.IsNullOrWhiteSpace(request.Category))
            {
                var category = request.Category.ToLower();
                predicate = CombineExpressions(predicate, t => t.Category.ToLower().Contains(category));
            }
            
            if (!string.IsNullOrWhiteSpace(request.SourceCountry))
            {
                var country = request.SourceCountry.ToUpper();
                predicate = CombineExpressions(predicate, t => t.SourceCountry != null && t.SourceCountry.Code == country);
            }
            
            if (!string.IsNullOrWhiteSpace(request.Asn))
            {
                var asn = request.Asn;
                predicate = CombineExpressions(predicate, t => t.AsnRegistry.Asn.Contains(asn));
            }

            // Build order by expression
            Expression<Func<ThreatEvent, object>>? orderBy = request.SortBy?.ToLower() switch
            {
                "timestamp" => t => t.Timestamp,
                "category" => t => t.Category,
                "sourceip" => t => t.SourceAddress.ToString(),
                "asn" => t => t.AsnRegistry.Asn,
                _ => t => t.Timestamp
            };

            var (items, totalCount) = await _repository.GetPaginatedAsync(
                predicate: predicate,
                orderBy: orderBy,
                orderByDescending: request.SortDescending,
                skip: (request.Page - 1) * request.PageSize,
                take: request.PageSize,
                ct: ct);

            // Load the items with includes separately
            var itemIds = items.Select(i => i.Id).ToList();
            var itemsWithIncludes = await _repository.Query()
                .Where(t => itemIds.Contains(t.Id))
                .Include(t => t.AsnRegistry)
                .Include(t => t.SourceCountry)
                .Include(t => t.DestinationCountry)
                .Include(t => t.Protocol)
                .Include(t => t.MalwareFamily)
                .ToListAsync(ct);

            var records = itemsWithIncludes.Select(t => new ThreatRecordDto
            {
                Id = t.Id,
                Timestamp = t.Timestamp,
                SourceIp = t.SourceAddress.ToString(),
                DestinationIp = t.DestinationAddress?.ToString(),
                Asn = t.AsnRegistry.Asn,
                AsnName = t.AsnRegistry.Description,
                SourceCountryCode = t.SourceCountry?.Code,
                SourceCountryName = t.SourceCountry?.Name,
                DestinationCountryCode = t.DestinationCountry?.Code,
                DestinationCountryName = t.DestinationCountry?.Name,
                ThreatCategory = t.Category,
                SourcePort = t.SourcePort,
                DestinationPort = t.DestinationPort,
                Protocol = t.Protocol?.Name,
                MalwareFamily = t.MalwareFamily?.Name,
                Severity = GetThreatSeverity(t.Category)
            });

            var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

            return new PaginatedThreatIntelligenceDto
            {
                Data = records,
                TotalRecords = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = totalPages,
                HasNextPage = request.Page < totalPages,
                HasPreviousPage = request.Page > 1
            };
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error retrieving paginated threat intelligence: {ex.Message}", ex);
        }
    }

    private static string GetThreatSeverity(string category)
    {
        return category.ToLowerInvariant() switch
        {
            "malware" => "High",
            "botnet" => "High",
            "phishing" => "High",
            "spam" => "Low",
            "scanning" => "Medium",
            "bruteforce" => "Medium",
            _ => "Medium"
        };
    }

    private static Expression<Func<ThreatEvent, bool>> CombineExpressions(
        Expression<Func<ThreatEvent, bool>> first,
        Expression<Func<ThreatEvent, bool>> second)
    {
        var parameter = Expression.Parameter(typeof(ThreatEvent), "t");
        var firstBody = ReplaceParameter(first.Body, first.Parameters[0], parameter);
        var secondBody = ReplaceParameter(second.Body, second.Parameters[0], parameter);
        var combined = Expression.AndAlso(firstBody, secondBody);
        return Expression.Lambda<Func<ThreatEvent, bool>>(combined, parameter);
    }

    private static Expression ReplaceParameter(Expression expression, ParameterExpression oldParameter, ParameterExpression newParameter)
    {
        return new ParameterReplacer(oldParameter, newParameter).Visit(expression);
    }

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