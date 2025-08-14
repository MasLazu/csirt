using MediatR;

namespace MeUi.Application.Models;

public abstract record BasePaginatedQuery<TResponse> : IRequest<PaginatedDto<TResponse>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Search { get; set; }
    public string? SortBy { get; set; }
    public string SortDirection { get; set; } = "asc";
    public bool IsDescending => SortDirection?.ToLower() == "desc";
    public int ValidatedPageSize => Math.Min(Math.Max(PageSize, 1), 100);
    public int ValidatedPage => Math.Max(Page, 1);
}
