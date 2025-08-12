using MediatR;
using MeUi.Application.Models;

namespace MeUi.Application.Features.Authorization.Queries.GetUserRoles;

public record GetUserRolesQuery : IRequest<IEnumerable<RoleDto>>
{
    public Guid UserId { get; set; }
}
