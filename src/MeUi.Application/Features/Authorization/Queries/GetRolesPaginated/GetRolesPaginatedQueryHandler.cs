using Mapster;
using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;
using MeUi.Domain.Entities;
using System.Linq.Expressions;

namespace MeUi.Application.Features.Authorization.Queries.GetRolesPaginated;

public class GetRolesPaginatedQueryHandler : IRequestHandler<GetRolesPaginatedQuery, PaginatedDto<RoleDto>>
{
    private readonly IRepository<Role> _roleRepository;

    public GetRolesPaginatedQueryHandler(IRepository<Role> roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<PaginatedDto<RoleDto>> Handle(GetRolesPaginatedQuery request, CancellationToken ct)
    {
        Expression<Func<Role, bool>>? predicate = null;

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            predicate = r => r.Name.Contains(request.Search) || r.Description.Contains(request.Search);
        }

        (IEnumerable<Role> roles, int totalItems) = await _roleRepository.GetPaginatedAsync(
            predicate: predicate,
            orderBy: r => r.Name,
            skip: (request.Page - 1) * request.PageSize,
            take: request.PageSize,
            ct: ct);

        int totalPages = (int)Math.Ceiling((double)totalItems / request.PageSize);

        return new PaginatedDto<RoleDto>
        {
            Items = roles.Adapt<List<RoleDto>>(),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalItems = totalItems,
            TotalPages = totalPages
        };
    }
}