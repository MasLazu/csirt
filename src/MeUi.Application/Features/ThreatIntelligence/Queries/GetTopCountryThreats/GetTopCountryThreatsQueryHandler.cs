using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Exceptions;
using MeUi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MeUi.Application.Features.ThreatIntelligence.Models;

namespace MeUi.Application.Features.ThreatIntelligence.Queries.GetTopCountryThreats;

public class GetTopCountryThreatsQueryHandler : IRequestHandler<GetTopCountryThreatsQuery, TopCountryThreatsDto>
{
    private readonly IRepository<ThreatEvent> _repository;
    private readonly ITenantContext _tenantContext;

    public GetTopCountryThreatsQueryHandler(
        IRepository<ThreatEvent> repository,
        ITenantContext tenantContext)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
    }

    public async Task<TopCountryThreatsDto> Handle(GetTopCountryThreatsQuery request, CancellationToken ct)
    {
        // Validate tenant context - only authenticated users with valid tenant context can access threat intelligence
        if (!_tenantContext.IsSuperAdmin && !_tenantContext.TenantId.HasValue)
        {
            throw new TenantAccessDeniedException();
        }

        try
        {
            // Ensure DateTime parameters are in UTC for PostgreSQL compatibility
            var startDateUtc = request.StartDate.Kind == DateTimeKind.Utc
                ? request.StartDate
                : request.StartDate.ToUniversalTime();
            var endDateUtc = request.EndDate.Kind == DateTimeKind.Utc
                ? request.EndDate
                : request.EndDate.ToUniversalTime();

            // Get top country threats using LINQ
            var topCountries = await _repository.Query()
                .Where(t => t.Timestamp >= startDateUtc && t.Timestamp <= endDateUtc && t.SourceCountry != null)
                .Include(t => t.SourceCountry)
                .GroupBy(t => new { t.SourceCountry!.Code, t.SourceCountry.Name })
                .Select(g => new CountryThreatCountDto
                {
                    CountryCode = g.Key.Code,
                    CountryName = g.Key.Name,
                    ThreatCount = g.Count()
                })
                .OrderByDescending(c => c.ThreatCount)
                .Take(request.Limit)
                .ToListAsync(ct);

            return new TopCountryThreatsDto
            {
                TopCountries = topCountries,
                StartDate = startDateUtc,
                EndDate = endDateUtc,
                Limit = request.Limit,
                TotalCountries = topCountries.Count()
            };
        }
        catch (TimeoutException ex)
        {
            throw new InvalidOperationException($"Top country threats query timeout: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error retrieving top country threats: {ex.Message}", ex);
        }
    }
}