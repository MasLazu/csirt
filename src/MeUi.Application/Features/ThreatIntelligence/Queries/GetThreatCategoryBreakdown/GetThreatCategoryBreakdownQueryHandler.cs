using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Exceptions;
using MeUi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MeUi.Application.Features.ThreatIntelligence.Models;

namespace MeUi.Application.Features.ThreatIntelligence.Queries.GetThreatCategoryBreakdown;

public class GetThreatCategoryBreakdownQueryHandler : IRequestHandler<GetThreatCategoryBreakdownQuery, ThreatCategoryBreakdownDto>
{
    private readonly IThreatEventRepository _repository;
    private readonly ITenantContext _tenantContext;

    public GetThreatCategoryBreakdownQueryHandler(
        IThreatEventRepository repository,
        ITenantContext tenantContext)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
    }

    public async Task<ThreatCategoryBreakdownDto> Handle(GetThreatCategoryBreakdownQuery request, CancellationToken ct)
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

            // Get category breakdown
            var categories = await _repository.GetThreatCountsByCategoryAsync(
                startDateUtc,
                endDateUtc,
                ct);

            // Calculate total threats across all categories
            var totalThreats = categories.Sum(c => c.ThreatCount);

            return new ThreatCategoryBreakdownDto
            {
                Categories = categories,
                StartDate = startDateUtc,
                EndDate = endDateUtc,
                TotalCategories = categories.Count(),
                TotalThreats = totalThreats
            };
        }
        catch (TimeoutException ex)
        {
            throw new InvalidOperationException($"Category breakdown query timeout: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error retrieving category breakdown: {ex.Message}", ex);
        }
    }
}