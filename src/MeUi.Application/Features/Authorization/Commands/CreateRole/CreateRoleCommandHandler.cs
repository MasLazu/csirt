using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.Authorization.Commands.CreateRole;

public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, Guid>
{
    private readonly IRepository<Role> _roleRepository;
    private readonly IRepository<RolePermission> _rolePermissionRepository;
    private readonly IRepository<Permission> _permissionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateRoleCommandHandler(
        IRepository<Role> roleRepository,
        IRepository<RolePermission> rolePermissionRepository,
        IRepository<Permission> permissionRepository,
        IUnitOfWork unitOfWork)
    {
        _roleRepository = roleRepository;
        _rolePermissionRepository = rolePermissionRepository;
        _permissionRepository = permissionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateRoleCommand request, CancellationToken ct)
    {
        var role = new Role
        {
            Name = request.Name,
            Description = request.Description
        };

        if (await _permissionRepository.CountAsync(p => request.PermissionIds.Contains(p.Id), ct) != request.PermissionIds.Count())
        {
            throw new BadRequestException("One or more permissions do not exist.");
        }

        await _roleRepository.AddAsync(role, ct);

        if (request.PermissionIds.Any())
        {
            foreach (Guid permissionId in request.PermissionIds)
            {
                var rolePermission = new RolePermission
                {
                    RoleId = role.Id,
                    PermissionId = permissionId
                };
                await _rolePermissionRepository.AddAsync(rolePermission, ct);
            }
        }

        await _unitOfWork.SaveChangesAsync(ct);

        return role.Id;
    }
}