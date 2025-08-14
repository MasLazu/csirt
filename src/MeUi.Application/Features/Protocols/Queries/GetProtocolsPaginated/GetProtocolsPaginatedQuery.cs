using MediatR;
using MeUi.Application.Models;

namespace MeUi.Application.Features.Protocols.Queries.GetProtocolsPaginated;

public class GetProtocolsPaginatedQuery : IRequest<PaginatedDto<ProtocolDto>>, ITenantRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 25;
    public string? Search { get; set; }
    public string? SortBy { get; set; }
    public bool IsDescending { get; set; }
    public Guid TenantId { get; set; }

    public int ValidatedPage => Page < 1 ? 1 : Page;
    public int ValidatedPageSize => PageSize switch
    {
        < 1 => 1,
        > 100 => 100,
        _ => PageSize
    };
}
