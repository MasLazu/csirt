using MediatR;
using MeUi.Application.Models;

namespace MeUi.Application.Features.Authorization.Queries.GetRoleById;

public class GetRoleByIdQuery : IRequest<RoleDto>
{
    public Guid Id { get; set; }
}