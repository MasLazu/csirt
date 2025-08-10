using MediatR;
using MeUi.Application.Features.ThreatIntelligence.Models;

namespace MeUi.Application.Features.ThreatIntelligence.Queries.GetThreatCategoryBreakdown;

public record GetThreatCategoryBreakdownQuery : IRequest<ThreatCategoryBreakdownDto>
{
    /// <summary>
    /// Start date for category breakdown calculation
    /// </summary>
    public DateTime StartDate { get; init; } = DateTime.UtcNow.AddDays(-7);

    /// <summary>
    /// End date for category breakdown calculation
    /// </summary>
    public DateTime EndDate { get; init; } = DateTime.UtcNow;
}

public class ThreatCategoryBreakdownDto
{
    public IEnumerable<CategoryThreatCountDto> Categories { get; set; } = new List<CategoryThreatCountDto>();
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalCategories { get; set; }
    public long TotalThreats { get; set; }
}