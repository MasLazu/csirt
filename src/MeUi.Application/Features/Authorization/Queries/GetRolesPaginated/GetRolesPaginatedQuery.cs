using MediatR;
using MeUi.Application.Features.Authorization.Models;
using MeUi.Application.Models;

namespace MeUi.Application.Features.Authorization.Queries.GetRolesPaginated;

public record GetRolesPaginatedQuery : IRequest<PaginatedResult<RoleDto>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? Search { get; init; }
}