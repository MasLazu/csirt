using MediatR;

namespace MeUi.Application.Models;

/// <summary>
/// Base class for paginated queries with standardized parameters
/// </summary>
public abstract record BasePaginatedQuery<TResponse> : IRequest<PaginatedDto<TResponse>>
{
    /// <summary>
    /// Page number (1-based)
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Number of items per page (default: 10, max: 100)
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Search term to filter results
    /// </summary>
    public string? Search { get; set; }

    /// <summary>
    /// Sort field (default: "Name" or entity-specific default)
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Sort direction: "asc" or "desc" (default: "asc")
    /// </summary>
    public string SortDirection { get; set; } = "asc";

    /// <summary>
    /// Validates if the sort direction is valid
    /// </summary>
    public bool IsDescending => SortDirection?.ToLower() == "desc";

    /// <summary>
    /// Ensures PageSize is within valid bounds
    /// </summary>
    public int ValidatedPageSize => Math.Min(Math.Max(PageSize, 1), 100);

    /// <summary>
    /// Ensures Page is at least 1
    /// </summary>
    public int ValidatedPage => Math.Max(Page, 1);
}
