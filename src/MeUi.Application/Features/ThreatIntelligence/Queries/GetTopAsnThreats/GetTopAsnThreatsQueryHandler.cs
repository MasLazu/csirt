using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Exceptions;
using MeUi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MeUi.Application.Features.ThreatIntelligence.Models;

namespace MeUi.Application.Features.ThreatIntelligence.Queries.GetTopAsnThreats;

public class GetTopAsnThreatsQueryHandler : IRequestHandler<GetTopAsnThreatsQuery, TopAsnThreatsDto>
{
    private readonly IRepository<ThreatEvent> _repository;
    private readonly ITenantContext _tenantContext;

    public GetTopAsnThreatsQueryHandler(
        IRepository<ThreatEvent> repository,
        ITenantContext tenantContext)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
    }

    public async Task<TopAsnThreatsDto> Handle(GetTopAsnThreatsQuery request, CancellationToken ct)
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

            // Get top ASN threats using LINQ
            var topAsns = await _repository.Query()
                .Where(t => t.Timestamp >= startDateUtc && t.Timestamp <= endDateUtc)
                .Include(t => t.AsnRegistry)
                .GroupBy(t => new { t.AsnRegistry.Asn, t.AsnRegistry.Description })
                .Select(g => new AsnThreatCountDto
                {
                    Asn = g.Key.Asn,
                    AsnDescription = g.Key.Description,
                    ThreatCount = g.Count()
                })
                .OrderByDescending(a => a.ThreatCount)
                .Take(request.Limit)
                .ToListAsync(ct);

            return new TopAsnThreatsDto
            {
                TopAsns = topAsns,
                StartDate = startDateUtc,
                EndDate = endDateUtc,
                Limit = request.Limit,
                TotalAsns = topAsns.Count()
            };
        }
        catch (TimeoutException ex)
        {
            throw new InvalidOperationException($"Top ASN threats query timeout: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error retrieving top ASN threats: {ex.Message}", ex);
        }
    }
}