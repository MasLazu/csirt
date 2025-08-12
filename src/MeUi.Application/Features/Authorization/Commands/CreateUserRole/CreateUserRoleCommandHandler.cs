using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.Authorization.Commands.CreateUserRole;

public class CreateUserRoleCommandHandler : IRequestHandler<CreateUserRoleCommand, Guid>
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Role> _roleRepository;
    private readonly IRepository<UserRole> _userRoleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserRoleCommandHandler(
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

    public async Task<Guid> Handle(CreateUserRoleCommand request, CancellationToken ct)
    {
        if (!await _userRepository.ExistsAsync(u => u.Id == request.UserId, ct))
        {
            throw new NotFoundException($"User with ID '{request.UserId}' not found");
        }

        if (!await _roleRepository.ExistsAsync(r => r.Id == request.RoleId, ct))
        {
            throw new NotFoundException($"Role with ID '{request.RoleId}' not found");
        }

        if (await _userRoleRepository.ExistsAsync(ur => ur.UserId == request.UserId && ur.RoleId == request.RoleId, ct))
        {
            throw new ConflictException("User already has this role assigned");
        }

        var userRole = new UserRole
        {
            UserId = request.UserId,
            RoleId = request.RoleId
        };

        await _userRoleRepository.AddAsync(userRole, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return userRole.Id;
    }
}
