using MediatR;
using MeUi.Application.Features.Tenants.Models;
using MeUi.Application.Models;

namespace MeUi.Application.Features.Tenants.Queries.GetTenantsPaginated;

public record GetTenantsPaginatedQuery : IRequest<PaginatedResult<TenantDto>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchTerm { get; init; }
    public bool? IsActive { get; init; }
}