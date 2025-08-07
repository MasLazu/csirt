using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;


namespace MeUi.Application.Features.Authorization.Commands.AssignUserRoles;

public class AssignUserRolesCommandHandler : IRequestHandler<AssignUserRolesCommand, IEnumerable<Guid>>
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Role> _roleRepository;
    private readonly IRepository<UserRole> _userRoleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignUserRolesCommandHandler(
        IRepository<User> userRepository,
        IRepository<Role> roleRepository,
        IRepository<UserRole> userRoleRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Guid>> Handle(AssignUserRolesCommand request, CancellationToken ct)
    {
        User user = await _userRepository.GetByIdAsync(request.UserId, ct) ??
            throw new NotFoundException($"User with ID '{request.UserId}' not found");

        IEnumerable<Role> roles = await _roleRepository.FindAsync(r => request.RoleIds.Contains(r.Id), ct);
        if (roles.Count() != request.RoleIds.Count())
        {
            throw new NotFoundException("One or more roles not found");
        }

        IEnumerable<UserRole> userRoles = await _userRoleRepository.FindAsync(ur => ur.UserId == request.UserId, ct);

        IEnumerable<UserRole> rolesToAdd = request.RoleIds
            .Where(roleId => !userRoles.Any(ur => ur.RoleId == roleId))
            .Select(roleId => new UserRole
            {
                UserId = request.UserId,
                RoleId = roleId
            });

        IEnumerable<UserRole> rolesToRemove = userRoles.Where(ur => !request.RoleIds.Contains(ur.RoleId));

        await _userRoleRepository.DeleteRangeAsync(rolesToRemove, ct);
        await _userRoleRepository.AddRangeAsync(rolesToAdd, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return request.RoleIds;
    }
}