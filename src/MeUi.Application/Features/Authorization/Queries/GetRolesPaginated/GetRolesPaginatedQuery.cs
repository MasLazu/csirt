using MediatR;
using MeUi.Application.Models;

namespace MeUi.Application.Features.Authorization.Queries.GetRolesPaginated;

public record GetRolesPaginatedQuery : IRequest<PaginatedDto<RoleDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Search { get; set; }
}