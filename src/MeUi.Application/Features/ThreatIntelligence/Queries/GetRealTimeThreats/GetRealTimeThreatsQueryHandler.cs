using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Features.ThreatIntelligence.Models;
using MeUi.Application.Exceptions;
using MeUi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MeUi.Application.Features.ThreatIntelligence.Queries.GetRealTimeThreats;

public class GetRealTimeThreatsQueryHandler : IRequestHandler<GetRealTimeThreatsQuery, RealTimeThreatsDto>
{
    private readonly IRepository<ThreatEvent> _repository;
    private readonly ITenantContext _tenantContext;

    public GetRealTimeThreatsQueryHandler(
        IRepository<ThreatEvent> repository,
        ITenantContext tenantContext)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
    }

    public async Task<RealTimeThreatsDto> Handle(GetRealTimeThreatsQuery request, CancellationToken ct)
    {
        // Validate tenant context - only authenticated users with valid tenant context can access threat intelligence
        if (!_tenantContext.IsSuperAdmin && !_tenantContext.TenantId.HasValue)
        {
            throw new TenantAccessDeniedException();
        }

        try
        {
            var now = DateTime.UtcNow;
            var oneHourAgo = now.AddHours(-1);
            var oneMinuteAgo = now.AddMinutes(-1);

            // Build filter predicate
            Expression<Func<ThreatEvent, bool>> predicate = t => t.Timestamp >= oneHourAgo && t.Timestamp <= now;
            
            if (!string.IsNullOrWhiteSpace(request.Category))
            {
                var category = request.Category.ToLower();
                predicate = CombineExpressions(predicate, t => t.Category.ToLower().Contains(category));
            }

            var recentThreats = await _repository.Query()
                .Where(predicate)
                .Include(t => t.AsnRegistry)
                .Include(t => t.SourceCountry)
                .Include(t => t.DestinationCountry)
                .Include(t => t.Protocol)
                .Include(t => t.MalwareFamily)
                .OrderByDescending(t => t.Timestamp)
                .Take(request.Limit)
                .ToListAsync(ct);

            var threats = recentThreats.Select(t => new RealTimeThreatDto
            {
                Id = t.Id,
                Timestamp = t.Timestamp,
                SourceIp = t.SourceAddress.ToString(),
                DestinationIp = t.DestinationAddress?.ToString(),
                SourceCountry = t.SourceCountry?.Name ?? "Unknown",
                Category = t.Category,
                Severity = GetThreatSeverity(t.Category),
                Asn = t.AsnRegistry.Asn,
                SourcePort = t.SourcePort,
                DestinationPort = t.DestinationPort,
                Protocol = t.Protocol?.Name,
                Description = GenerateThreatDescription(t.Category, t.SourceAddress.ToString(), t.SourceCountry?.Name),
                IsActive = (now - t.Timestamp).TotalMinutes < 5
            });

            // Get stats using count queries
            var threatsLastHour = await _repository.CountAsync(t => t.Timestamp >= oneHourAgo, ct);
            var threatsLastMinute = await _repository.CountAsync(t => t.Timestamp >= oneMinuteAgo, ct);

            // Calculate average per minute
            var averagePerMinute = threatsLastHour / 60.0;

            // Get most active country and category from recent threats
            var mostActiveCountry = threats
                .Where(t => !string.IsNullOrEmpty(t.SourceCountry) && t.SourceCountry != "Unknown")
                .GroupBy(t => t.SourceCountry)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault()?.Key ?? "Unknown";

            var mostCommonCategory = threats
                .Where(t => !string.IsNullOrEmpty(t.Category))
                .GroupBy(t => t.Category)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault()?.Key ?? "Unknown";

            // Estimate active sources (unique source IPs in last hour)
            var activeSources = threats.Select(t => t.SourceIp).Distinct().Count();

            return new RealTimeThreatsDto
            {
                Threats = threats,
                TotalCount = threats.Count(),
                LastUpdated = now,
                Stats = new RealTimeStatsDto
                {
                    ThreatsLastHour = threatsLastHour,
                    ThreatsLastMinute = threatsLastMinute,
                    AverageThreatsPerMinute = Math.Round(averagePerMinute, 2),
                    MostActiveCountry = mostActiveCountry,
                    MostCommonCategory = mostCommonCategory,
                    ActiveSources = activeSources
                }
            };
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error retrieving real-time threat data: {ex.Message}", ex);
        }
    }

    private static string GetThreatSeverity(string category)
    {
        return category?.ToLower() switch
        {
            "malware" or "ransomware" or "trojan" => "Critical",
            "botnet" or "ddos" or "bruteforce" => "High",
            "phishing" or "spam" => "Medium",
            "scanning" or "reconnaissance" => "Low",
            _ => "Medium"
        };
    }

    private static string GenerateThreatDescription(string category, string sourceIp, string? sourceCountry)
    {
        var countryPart = !string.IsNullOrEmpty(sourceCountry) && sourceCountry != "Unknown" 
            ? $" from {sourceCountry}" 
            : "";
            
        return category?.ToLower() switch
        {
            "malware" => $"Malware activity detected from {sourceIp}{countryPart}",
            "botnet" => $"Botnet communication detected from {sourceIp}{countryPart}",
            "phishing" => $"Phishing attempt detected from {sourceIp}{countryPart}",
            "ddos" => $"DDoS attack detected from {sourceIp}{countryPart}",
            "bruteforce" => $"Brute force attack detected from {sourceIp}{countryPart}",
            "scanning" => $"Port scanning activity detected from {sourceIp}{countryPart}",
            _ => $"Suspicious activity detected from {sourceIp}{countryPart}"
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