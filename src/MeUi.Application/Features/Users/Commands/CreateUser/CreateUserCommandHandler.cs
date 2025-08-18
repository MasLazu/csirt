using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<TenantUser> _tenantUserRepository;
    private readonly IRepository<Password> _passwordRepository;
    private readonly IRepository<UserPassword> _userPasswordRepository;
    private readonly IRepository<UserLoginMethod> _userLoginMethodRepository;
    private readonly IRepository<UserRole> _userRoleRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IRepository<Role> _roleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserCommandHandler(
        IRepository<User> userRepository,
        IRepository<TenantUser> tenantUserRepository,
        IRepository<UserLoginMethod> userLoginMethodRepository,
        IRepository<Password> passwordRepository,
        IRepository<UserPassword> userPasswordRepository,
        IRepository<UserRole> userRoleRepository,
        IRepository<Role> roleRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _tenantUserRepository = tenantUserRepository;
        _passwordHasher = passwordHasher;
        _userLoginMethodRepository = userLoginMethodRepository;
        _passwordRepository = passwordRepository;
        _userPasswordRepository = userPasswordRepository;
        _userRoleRepository = userRoleRepository;
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken ct)
    {
        if (await _userRepository.ExistsAsync(u => u.Email == request.Email || u.Username == request.Username, ct))
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

        var user = new User
        {
            Email = request.Email,
            Username = request.Username,
            Name = request.Name,
        };

        var userLoginMethod = new UserLoginMethod
        {
            UserId = user.Id,
            LoginMethodCode = "PASSWORD"
        };

        var password = new Password
        {
            PasswordHash = _passwordHasher.HashPassword(request.Password)
        };

        var userPassword = new UserPassword
        {
            PasswordId = password.Id,
            UserLoginMethodId = userLoginMethod.Id
        };

        await _userRepository.AddAsync(user, ct);
        await _userLoginMethodRepository.AddAsync(userLoginMethod, ct);
        await _passwordRepository.AddAsync(password, ct);
        await _userPasswordRepository.AddAsync(userPassword, ct);

        if (request.RoleIds.Any())
        {
            foreach (Guid roleId in request.RoleIds)
            {
                var userRole = new UserRole
                {
                    UserId = user.Id,
                    RoleId = roleId
                };
                await _userRoleRepository.AddAsync(userRole, ct);
            }
        }

        await _unitOfWork.SaveChangesAsync(ct);

        return user.Id;
    }
}