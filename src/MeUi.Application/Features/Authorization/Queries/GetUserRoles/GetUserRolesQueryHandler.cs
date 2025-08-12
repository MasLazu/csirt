using Mapster;
using MediatR;
using MeUi.Application.Features.Authorization.Queries.GetUserRoles;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.Authorization.Queries.GetUserRoles;

public class GetUserRolesQueryHandler : IRequestHandler<GetUserRolesQuery, IEnumerable<RoleDto>>
{
    private readonly IRepository<UserRole> _userRoleRepository;
    private readonly IRepository<Role> _roleRepository;

    public GetUserRolesQueryHandler(
        IRepository<UserRole> userRoleRepository,
        IRepository<Role> roleRepository)
    {
        _userRoleRepository = userRoleRepository;
        _roleRepository = roleRepository;
    }

    public async Task<IEnumerable<RoleDto>> Handle(GetUserRolesQuery request, CancellationToken ct)
    {
        IEnumerable<Guid> roleIds = await _userRoleRepository.FindAsync(
            ur => ur.UserId == request.UserId,
            ur => ur.RoleId,
            ct);

        IEnumerable<Role> roles = await _roleRepository.FindAsync(r => roleIds.Contains(r.Id), ct: ct);

        return roles.Adapt<IEnumerable<RoleDto>>();
    }
}
