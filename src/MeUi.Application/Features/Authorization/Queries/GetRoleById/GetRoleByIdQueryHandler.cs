using Mapster;
using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;
using MeUi.Application.Exceptions;
using MeUi.Application.Models;

namespace MeUi.Application.Features.Authorization.Queries.GetRoleById;

public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, RoleDto>
{
    private readonly IRepository<Role> _roleRepository;

    public GetRoleByIdQueryHandler(IRepository<Role> roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<RoleDto> Handle(GetRoleByIdQuery request, CancellationToken ct)
    {
        Role role = await _roleRepository.GetByIdAsync(request.Id, ct) ??
            throw new NotFoundException($"Role with ID {request.Id} not found");

        return role.Adapt<RoleDto>();
    }
}