using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Guid>
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<TenantUser> _tenantUserRepository;
    private readonly IRepository<UserRole> _userRoleRepository;
    private readonly IRepository<Role> _roleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserCommandHandler(
        IRepository<User> userRepository,
        IRepository<TenantUser> tenantUserRepository,
        IRepository<UserRole> userRoleRepository,
        IRepository<Role> roleRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _tenantUserRepository = tenantUserRepository;
        _userRoleRepository = userRoleRepository;
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(UpdateUserCommand request, CancellationToken ct)
    {
        User user = await _userRepository.GetByIdAsync(request.Id, ct) ??
            throw new NotFoundException($"User with ID '{request.Id}' not found.");

        if (await _userRepository.ExistsAsync(u => (u.Email == request.Email || u.Username == request.Username) && u.Id != request.Id, ct))
        {
            throw new BadRequestException("User with this email already exists.");
        }

        if (await _tenantUserRepository.ExistsAsync(tu => tu.Email == request.Email || tu.Username == request.Username, ct))
        {
            throw new BadRequestException("Tenant user with this email already exists.");
        }

        if (await _roleRepository.CountAsync(r => request.RoleIds.Contains(r.Id), ct) != request.RoleIds.Count())
        {
            throw new BadRequestException("One or more roles do not exist.");
        }

        user.Username = request.Username;
        user.Email = request.Email;
        user.Name = request.Name;
        user.IsSuspended = request.IsSuspended;

        await _userRepository.UpdateAsync(user, ct);

        IEnumerable<UserRole> existingUserRoles = await _userRoleRepository.FindAsync(ur => ur.UserId == request.Id, ct);
        foreach (UserRole existingRole in existingUserRoles)
        {
            await _userRoleRepository.DeleteAsync(existingRole, ct);
        }

        if (request.RoleIds.Any())
        {
            foreach (Guid roleId in request.RoleIds)
            {
                var userRole = new UserRole
                {
                    UserId = request.Id,
                    RoleId = roleId
                };
                await _userRoleRepository.AddAsync(userRole, ct);
            }
        }

        await _unitOfWork.SaveChangesAsync(ct);

        return user.Id;
    }
}