using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MeUi.Infrastructure.Data;

namespace MeUi.Infrastructure.Data.Seeders;

public class SuperUserSeeder
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<LoginMethod> _loginMethodRepository;
    private readonly IRepository<UserLoginMethod> _userLoginMethodRepository;
    private readonly IRepository<Password> _passwordRepository;
    private readonly IRepository<UserRole> _userRoleRepository;
    private readonly IRepository<Role> _roleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SuperUserSeeder> _logger;

    public SuperUserSeeder(
        IRepository<User> userRepository,
        IRepository<LoginMethod> loginMethodRepository,
        IRepository<UserLoginMethod> userLoginMethodRepository,
        IRepository<Password> passwordRepository,
        IRepository<UserRole> userRoleRepository,
        IRepository<Role> roleRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IConfiguration configuration,
        ApplicationDbContext context,
        ILogger<SuperUserSeeder> logger)
    {
        _userRepository = userRepository;
        _loginMethodRepository = loginMethodRepository;
        _userLoginMethodRepository = userLoginMethodRepository;
        _passwordRepository = passwordRepository;
        _userRoleRepository = userRoleRepository;
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken ct = default)
    {
        string? superUserEmail = _configuration["SuperUser:Email"];
        string? superUserPassword = _configuration["SuperUser:Password"];

        if (string.IsNullOrEmpty(superUserEmail) || string.IsNullOrEmpty(superUserPassword))
        {
            _logger.LogWarning("SuperUser configuration is missing, skipping super user creation");
            return;
        }

        User? existingUser = await _userRepository.FirstOrDefaultAsync(x => x.Email == superUserEmail, ct);
        if (existingUser != null)
        {
            _logger.LogInformation("Super user already exists: {Email}", superUserEmail);
            return;
        }

        var superUser = new User
        {
            Email = superUserEmail,
            Username = _configuration["SuperUser:Username"] ?? "superadmin",
            Name = _configuration["SuperUser:Name"] ?? "Super Administrator",
            IsSuspended = false
        };

        await _userRepository.AddAsync(superUser, ct);
        _logger.LogInformation("Seeded super user: {Email}", superUserEmail);

        LoginMethod? passwordLoginMethod = await _loginMethodRepository.FirstOrDefaultAsync(x => x.Code == "PASSWORD", ct);
        if (passwordLoginMethod == null)
        {
            _logger.LogError("PASSWORD login method not found. Cannot create super user login credentials.");
            return;
        }

        var userLoginMethod = new UserLoginMethod
        {
            UserId = superUser.Id,
            LoginMethodCode = passwordLoginMethod.Code
        };

        await _userLoginMethodRepository.AddAsync(userLoginMethod, ct);
        _logger.LogInformation("Created password login method for super user");

        string hashedPassword = _passwordHasher.HashPassword(superUserPassword);
        var passwordRecord = new Password
        {
            PasswordHash = hashedPassword
        };

        await _passwordRepository.AddAsync(passwordRecord, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        _logger.LogInformation("Created password hash for super user");

        var userPassword = new UserPassword
        {
            PasswordId = passwordRecord.Id,
            UserLoginMethodId = userLoginMethod.Id
        };

        await _context.UserPasswords.AddAsync(userPassword, ct);
        _logger.LogInformation("Created user password association for super user");

        Role? superAdminRole = await _roleRepository.FirstOrDefaultAsync(x => x.Id == Guid.Parse("01989299-2c61-71a0-92b9-5a7700dd263e"), ct);
        if (superAdminRole != null)
        {
            UserRole? existingUserRole = await _userRoleRepository.FirstOrDefaultAsync(
                ur => ur.UserId == superUser.Id && ur.RoleId == superAdminRole.Id, ct);

            if (existingUserRole == null)
            {
                var userRole = new UserRole
                {
                    UserId = superUser.Id,
                    RoleId = superAdminRole.Id
                };

                await _userRoleRepository.AddAsync(userRole, ct);
                _logger.LogInformation("Assigned SUPER_ADMIN role to super user");
            }
        }
        else
        {
            _logger.LogWarning("SUPER_ADMIN role not found. Cannot assign role to super user.");
        }
    }
}
