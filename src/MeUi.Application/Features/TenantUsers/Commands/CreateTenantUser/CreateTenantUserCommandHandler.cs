using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.TenantUsers.Commands.CreateTenantUser;

public class CreateTenantUserCommandHandler : IRequestHandler<CreateTenantUserCommand, Guid>
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<TenantUser> _tenantUserRepository;
    private readonly IRepository<TenantRole> _tenantRoleRepository;
    private readonly IRepository<TenantUserRole> _tenantUserRoleRepository;
    private readonly IRepository<Password> _passwordRepository;
    private readonly IRepository<TenantUserPassword> _tenantUserPasswordRepository;
    private readonly IRepository<TenantUserLoginMethod> _tenantUserLoginMethodRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTenantUserCommandHandler(
        IRepository<User> userRepository,
        IRepository<TenantUser> tenantUserRepository,
        IRepository<TenantRole> tenantRoleRepository,
        IRepository<TenantUserRole> tenantUserRoleRepository,
        IRepository<Password> passwordRepository,
        IRepository<TenantUserPassword> tenantUserPasswordRepository,
        IRepository<TenantUserLoginMethod> tenantUserLoginMethodRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _tenantRoleRepository = tenantRoleRepository;
        _tenantUserRoleRepository = tenantUserRoleRepository;
        _tenantUserRepository = tenantUserRepository;
        _passwordRepository = passwordRepository;
        _tenantUserPasswordRepository = tenantUserPasswordRepository;
        _tenantUserLoginMethodRepository = tenantUserLoginMethodRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateTenantUserCommand request, CancellationToken ct)
    {
        if (await _userRepository.ExistsAsync(u => u.Email == request.Email || u.Username == request.Username, ct))
        {
            throw new ConflictException("User with this email already exists.");
        }

        if (await _tenantUserRepository.ExistsAsync(tu => tu.Email == request.Email && tu.TenantId == request.TenantId, ct))
        {
            throw new ConflictException("Tenant user with this email already exists.");
        }

        if (await _tenantRoleRepository.CountAsync(tr => request.RoleIds.Contains(tr.Id), ct) != request.RoleIds.Count)
        {
            throw new NotFoundException("One or more roles not found.");
        }

        var tenantUser = new TenantUser
        {
            Email = request.Email,
            TenantId = request.TenantId,
            Username = request.Username,
            Name = request.Name,
        };

        var password = new Password
        {
            PasswordHash = _passwordHasher.HashPassword(request.Password)
        };

        var tenantUserLoginMethod = new TenantUserLoginMethod
        {
            TenantUserId = tenantUser.Id,
            LoginMethodCode = "PASSWORD"
        };

        var tenantUserPassword = new TenantUserPassword
        {
            TenantUserLoginMethodId = tenantUserLoginMethod.Id,
            PasswordId = password.Id
        };

        var tenantUserROle = request.RoleIds.Select(roleId => new TenantUserRole
        {
            TenantUserId = tenantUser.Id,
            TenantRoleId = roleId
        }).ToList();

        await _tenantUserRepository.AddAsync(tenantUser, ct);
        await _tenantUserRoleRepository.AddRangeAsync(tenantUserROle, ct);
        await _passwordRepository.AddAsync(password, ct);
        await _tenantUserLoginMethodRepository.AddAsync(tenantUserLoginMethod, ct);
        await _tenantUserPasswordRepository.AddAsync(tenantUserPassword, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return tenantUser.Id;
    }
}