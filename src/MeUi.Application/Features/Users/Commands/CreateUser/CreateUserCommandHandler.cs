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
    private readonly IRepository<UserLoginMethod> _userLoginMethodRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserCommandHandler(
        IRepository<User> userRepository,
        IRepository<TenantUser> tenantUserRepository,
        IRepository<UserLoginMethod> userLoginMethodRepository,
        IRepository<Password> passwordRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _tenantUserRepository = tenantUserRepository;
        _passwordHasher = passwordHasher;
        _userLoginMethodRepository = userLoginMethodRepository;
        _passwordRepository = passwordRepository;
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
            PasswordHash = _passwordHasher.HashPassword(request.Password),
        };

        await _userRepository.AddAsync(user, ct);
        await _userLoginMethodRepository.AddAsync(userLoginMethod, ct);
        await _passwordRepository.AddAsync(password, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return user.Id;
    }
}