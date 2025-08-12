using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;
using MeUi.Application.Exceptions;

namespace MeUi.Application.Features.Authorization.Commands.PutUserRoles;

public class PutUserRolesCommandHandler : IRequestHandler<PutUserRolesCommand, IEnumerable<Guid>>
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Role> _roleRepository;
    private readonly IRepository<UserRole> _userRoleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PutUserRolesCommandHandler(
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

    public async Task<IEnumerable<Guid>> Handle(PutUserRolesCommand request, CancellationToken ct)
    {
        User user = await _userRepository.GetByIdAsync(request.UserId, ct) ??
            throw new NotFoundException($"User with ID '{request.UserId}' not found");

        IEnumerable<Role> roles = await _roleRepository.FindAsync(r => request.RoleIds.Contains(r.Id), ct);
        if (roles.Count() != request.RoleIds.Count())
        {
            throw new NotFoundException("One or more roles not found");
        }

        IEnumerable<UserRole> existingUserRoles = await _userRoleRepository.FindAsync(ur => ur.UserId == request.UserId, ct);

        await _userRoleRepository.DeleteRangeAsync(existingUserRoles, ct);

        IEnumerable<UserRole> newUserRoles = request.RoleIds.Select(roleId => new UserRole
        {
            UserId = request.UserId,
            RoleId = roleId
        });

        await _userRoleRepository.AddRangeAsync(newUserRoles, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return request.RoleIds;
    }
}
