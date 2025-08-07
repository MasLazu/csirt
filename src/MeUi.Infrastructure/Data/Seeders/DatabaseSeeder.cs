using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;
using System.Reflection;

namespace MeUi.Infrastructure.Data.Seeders;

public class DatabaseSeeder
{
    private readonly IRepository<LoginMethod> _loginMethodRepository;
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Role> _roleRepository;
    private readonly IRepository<Resource> _resourceRepository;
    private readonly IRepository<Domain.Entities.Action> _actionRepository;
    private readonly IRepository<PageGroup> _pageGroupRepository;
    private readonly IRepository<Page> _pageRepository;
    private readonly IRepository<Permission> _permissionRepository;
    private readonly IRepository<PagePermission> _pagePermissionRepository;
    private readonly IRepository<UserLoginMethod> _userLoginMethodRepository;
    private readonly IRepository<Password> _passwordRepository;
    private readonly IRepository<UserRole> _userRoleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(
        IRepository<LoginMethod> loginMethodRepository,
        IRepository<User> userRepository,
        IRepository<Role> roleRepository,
        IRepository<Resource> resourceRepository,
        IRepository<Domain.Entities.Action> actionRepository,
        IRepository<PageGroup> pageGroupRepository,
        IRepository<Page> pageRepository,
        IRepository<Permission> permissionRepository,
        IRepository<PagePermission> pagePermissionRepository,
        IRepository<UserLoginMethod> userLoginMethodRepository,
        IRepository<Password> passwordRepository,
        IRepository<UserRole> userRoleRepository,
        IUnitOfWork unitOfWork,
        IConfiguration configuration,
        IPasswordHasher passwordHasher,
        ILogger<DatabaseSeeder> logger)
    {
        _loginMethodRepository = loginMethodRepository;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _resourceRepository = resourceRepository;
        _actionRepository = actionRepository;
        _pageGroupRepository = pageGroupRepository;
        _pageRepository = pageRepository;
        _permissionRepository = permissionRepository;
        _pagePermissionRepository = pagePermissionRepository;
        _userLoginMethodRepository = userLoginMethodRepository;
        _passwordRepository = passwordRepository;
        _userRoleRepository = userRoleRepository;
        _unitOfWork = unitOfWork;
        _configuration = configuration;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken ct = default)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(ct);

            await SeedLoginMethodsAsync(ct);
            await _unitOfWork.SaveChangesAsync(ct);

            await SeedPermissionsAsync(ct);
            await _unitOfWork.SaveChangesAsync(ct);

            await SeedSuperRolesAsync(ct);
            await _unitOfWork.SaveChangesAsync(ct);

            await SeedPageGroupsAsync(ct);
            await _unitOfWork.SaveChangesAsync(ct);

            await SeedPagesAsync(ct);
            await _unitOfWork.SaveChangesAsync(ct);

            await SeedSuperUserAsync(ct);
            await _unitOfWork.SaveChangesAsync(ct);

            await _unitOfWork.CommitTransactionAsync(ct);

            _logger.LogInformation("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during database seeding");
            await _unitOfWork.RollbackTransactionAsync(ct);
            throw;
        }
    }

    private async Task SeedLoginMethodsAsync(CancellationToken ct)
    {
        var loginMethods = new[]
        {
            new { Code = "PASSWORD", Name = "Password", Description = "Username/Email and Password authentication" },
        };

        foreach (var method in loginMethods)
        {
            var existing = await _loginMethodRepository.FirstOrDefaultAsync(x => x.Code == method.Code, ct);
            if (existing == null)
            {
                await _loginMethodRepository.AddAsync(new LoginMethod
                {
                    Code = method.Code,
                    Name = method.Name,
                    Description = method.Description,
                    IsActive = true
                }, ct);

                _logger.LogInformation("Seeded login method: {Code}", method.Code);
            }
        }
    }

    private async Task SeedSuperRolesAsync(CancellationToken ct)
    {
        var role = new Role
        {
            Code = "SUPER_ADMIN",
            Name = "Super Administrator",
            Description = "Full system access"
        };

        var existing = await _roleRepository.FirstOrDefaultAsync(x => x.Code == role.Code, ct);
        if (existing == null)
        {
            await _roleRepository.AddAsync(role, ct);
            _logger.LogInformation("Seeded role: {Code}", role.Code);
        }
    }

    private async Task SeedSuperUserAsync(CancellationToken ct)
    {
        string? superUserEmail = _configuration["SuperUser:Email"];
        string? superUserPassword = _configuration["SuperUser:Password"];

        if (string.IsNullOrEmpty(superUserEmail) || string.IsNullOrEmpty(superUserPassword))
        {
            _logger.LogWarning("SuperUser configuration is missing, skipping super user creation");
            return;
        }

        var existingUser = await _userRepository.FirstOrDefaultAsync(x => x.Email == superUserEmail, ct);
        if (existingUser == null)
        {
            var superUser = new User
            {
                Email = superUserEmail,
                Username = _configuration["SuperUser:Username"] ?? "superadmin",
                Name = $"{_configuration["SuperUser:FirstName"]} {_configuration["SuperUser:LastName"]}".Trim(),
                IsSuspended = false
            };

            await _userRepository.AddAsync(superUser, ct);
            _logger.LogInformation("Seeded super user: {Email}", superUserEmail);

            var passwordLoginMethod = await _loginMethodRepository.FirstOrDefaultAsync(x => x.Code == "PASSWORD", ct);
            if (passwordLoginMethod == null)
            {
                _logger.LogError("PASSWORD login method not found. Cannot create super user login credentials.");
                return;
            }

            var userLoginMethod = new UserLoginMethod
            {
                UserId = superUser.Id,
                LoginMethodId = passwordLoginMethod.Id,
                LoginMethodCode = passwordLoginMethod.Code
            };

            await _userLoginMethodRepository.AddAsync(userLoginMethod, ct);
            _logger.LogInformation("Created password login method for super user");

            var hashedPassword = _passwordHasher.HashPassword(superUserPassword);
            var passwordRecord = new Password
            {
                UserLoginMethodId = userLoginMethod.Id,
                PasswordHash = hashedPassword
            };

            await _passwordRepository.AddAsync(passwordRecord, ct);
            _logger.LogInformation("Created password hash for super user");

            var superAdminRole = await _roleRepository.FirstOrDefaultAsync(x => x.Code == "SUPER_ADMIN", ct);
            if (superAdminRole != null)
            {
                var existingUserRole = await _userRoleRepository.FirstOrDefaultAsync(
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
        else
        {
            _logger.LogInformation("Super user already exists: {Email}", superUserEmail);
        }
    }

    private async Task SeedPageGroupsAsync(CancellationToken ct)
    {
        var pageGroups = new[]
        {
            new { Code = "DASHBOARD", Name = "Dashboard", Icon = "dashboard" },
            new { Code = "USER_MANAGEMENT", Name = "User Management", Icon = "people" },
            new { Code = "AUTHORIZATION", Name = "Authorization", Icon = "security" },
            new { Code = "SYSTEM", Name = "System", Icon = "settings" },
            new { Code = "REPORTS", Name = "Reports", Icon = "analytics" }
        };

        foreach (var group in pageGroups)
        {
            var existing = await _pageGroupRepository.FirstOrDefaultAsync(x => x.Code == group.Code, ct);
            if (existing == null)
            {
                await _pageGroupRepository.AddAsync(new PageGroup
                {
                    Code = group.Code,
                    Name = group.Name,
                    Icon = group.Icon
                }, ct);

                _logger.LogInformation("Seeded page group: {Code}", group.Code);
            }
        }
    }

    private async Task SeedPagesAsync(CancellationToken ct)
    {
        // Get page groups for foreign key references
        var dashboardGroup = await _pageGroupRepository.FirstOrDefaultAsync(x => x.Code == "DASHBOARD", ct);
        var userMgmtGroup = await _pageGroupRepository.FirstOrDefaultAsync(x => x.Code == "USER_MANAGEMENT", ct);
        var authGroup = await _pageGroupRepository.FirstOrDefaultAsync(x => x.Code == "AUTHORIZATION", ct);
        var systemGroup = await _pageGroupRepository.FirstOrDefaultAsync(x => x.Code == "SYSTEM", ct);
        var reportsGroup = await _pageGroupRepository.FirstOrDefaultAsync(x => x.Code == "REPORTS", ct);

        var pages = new[]
        {
            // Dashboard pages
            new { Code = "DASHBOARD_HOME", Name = "Dashboard", Path = "/dashboard", PageGroupId = dashboardGroup?.Id },
            new { Code = "DASHBOARD_ANALYTICS", Name = "Analytics", Path = "/dashboard/analytics", PageGroupId = dashboardGroup?.Id },

            // User Management pages
            new { Code = "USERS_LIST", Name = "Users List", Path = "/users", PageGroupId = userMgmtGroup?.Id },
            new { Code = "USERS_CREATE", Name = "Create User", Path = "/users/create", PageGroupId = userMgmtGroup?.Id },
            new { Code = "USERS_EDIT", Name = "Edit User", Path = "/users/edit", PageGroupId = userMgmtGroup?.Id },
            new { Code = "USERS_VIEW", Name = "View User", Path = "/users/view", PageGroupId = userMgmtGroup?.Id },

            // Authorization pages
            new { Code = "ROLES_LIST", Name = "Roles List", Path = "/roles", PageGroupId = authGroup?.Id },
            new { Code = "ROLES_CREATE", Name = "Create Role", Path = "/roles/create", PageGroupId = authGroup?.Id },
            new { Code = "ROLES_EDIT", Name = "Edit Role", Path = "/roles/edit", PageGroupId = authGroup?.Id },
            new { Code = "PERMISSIONS_LIST", Name = "Permissions List", Path = "/permissions", PageGroupId = authGroup?.Id },
            new { Code = "PAGES_LIST", Name = "Pages List", Path = "/pages", PageGroupId = authGroup?.Id },

            // System pages
            new { Code = "SYSTEM_SETTINGS", Name = "System Settings", Path = "/system/settings", PageGroupId = systemGroup?.Id },
            new { Code = "SYSTEM_LOGS", Name = "System Logs", Path = "/system/logs", PageGroupId = systemGroup?.Id },

            // Reports pages
            new { Code = "REPORTS_USER", Name = "User Reports", Path = "/reports/users", PageGroupId = reportsGroup?.Id },
            new { Code = "REPORTS_SYSTEM", Name = "System Reports", Path = "/reports/system", PageGroupId = reportsGroup?.Id }
        };

        foreach (var page in pages)
        {
            var existing = await _pageRepository.FirstOrDefaultAsync(x => x.Code == page.Code, ct);
            if (existing == null)
            {
                await _pageRepository.AddAsync(new Page
                {
                    Code = page.Code,
                    Name = page.Name,
                    Path = page.Path,
                    PageGroupId = page.PageGroupId
                }, ct);

                _logger.LogInformation("Seeded page: {Code}", page.Code);
            }
        }
    }

    private async Task SeedPermissionsAsync(CancellationToken ct)
    {
        _logger.LogInformation("Starting permission seeding from IPermissionProvider implementations");

        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
            .ToList();

        var permissionProviders = new List<Type>();

        foreach (var assembly in assemblies)
        {
            try
            {
                var types = assembly.GetTypes()
                    .Where(t => t.IsClass && !t.IsAbstract &&
                               t.GetInterfaces().Any(i => i == typeof(IPermissionProvider)))
                    .ToList();

                permissionProviders.AddRange(types);
            }
            catch (ReflectionTypeLoadException ex)
            {
                _logger.LogWarning("Could not load types from assembly {AssemblyName}: {Error}",
                    assembly.FullName, ex.Message);
                continue;
            }
        }

        _logger.LogInformation("Found {Count} permission provider classes", permissionProviders.Count);

        var discoveredPermissions = new HashSet<string>();

        foreach (var providerType in permissionProviders)
        {
            try
            {
                var permissionMethod = providerType.GetMethod("Permission",
                    BindingFlags.Public | BindingFlags.Static);

                if (permissionMethod != null)
                {
                    var permission = permissionMethod.Invoke(null, null) as string;
                    if (!string.IsNullOrEmpty(permission))
                    {
                        discoveredPermissions.Add(permission);
                        _logger.LogDebug("Discovered permission: {Permission} from {ProviderType}",
                            permission, providerType.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting permission from {ProviderType}", providerType.Name);
            }
        }

        _logger.LogInformation("Discovered {Count} unique permissions", discoveredPermissions.Count);

        // Parse and collect unique resources and actions
        var parsedPermissions = new List<(string ActionCode, string ResourceCode)>();
        var uniqueResources = new HashSet<string>();
        var uniqueActions = new HashSet<string>();

        foreach (var permissionString in discoveredPermissions)
        {
            var parts = permissionString.Split(':', 2);
            if (parts.Length != 2)
            {
                _logger.LogWarning("Invalid permission format: {Permission}. Expected format: 'action:resource'",
                    permissionString);
                continue;
            }

            var actionCode = parts[0].Trim().ToUpperInvariant();
            var resourceCode = parts[1].Trim().ToUpperInvariant();

            parsedPermissions.Add((actionCode, resourceCode));
            uniqueResources.Add(resourceCode);
            uniqueActions.Add(actionCode);
        }

        // Create resources first
        foreach (var resourceCode in uniqueResources)
        {
            var existing = await _resourceRepository.FirstOrDefaultAsync(r => r.Code == resourceCode, ct);
            if (existing == null)
            {
                var resource = new Resource
                {
                    Code = resourceCode,
                    Name = ToTitleCase(resourceCode.Replace("_", " ")),
                    Description = $"Resource for {resourceCode.ToLowerInvariant()}"
                };
                await _resourceRepository.AddAsync(resource, ct);
                _logger.LogInformation("Created resource: {ResourceCode}", resourceCode);
            }
        }

        // Create actions
        foreach (var actionCode in uniqueActions)
        {
            var existing = await _actionRepository.FirstOrDefaultAsync(a => a.Code == actionCode, ct);
            if (existing == null)
            {
                var action = new Domain.Entities.Action
                {
                    Code = actionCode,
                    Name = ToTitleCase(actionCode.Replace("_", " ")),
                    Description = $"Action for {actionCode.ToLowerInvariant()}"
                };
                await _actionRepository.AddAsync(action, ct);
                _logger.LogInformation("Created action: {ActionCode}", actionCode);
            }
        }

        // Create permissions
        foreach (var (actionCode, resourceCode) in parsedPermissions)
        {
            var existing = await _permissionRepository.FirstOrDefaultAsync(
                p => p.ResourceCode == resourceCode && p.ActionCode == actionCode, ct);

            if (existing == null)
            {
                var permission = new Permission
                {
                    ResourceCode = resourceCode,
                    ActionCode = actionCode
                };
                await _permissionRepository.AddAsync(permission, ct);
                _logger.LogInformation("Created permission: {ActionCode}:{ResourceCode}", actionCode, resourceCode);
            }
        }

        _logger.LogInformation("Permission seeding completed");
    }

    private static string ToTitleCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var words = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < words.Length; i++)
        {
            if (words[i].Length > 0)
            {
                words[i] = char.ToUpperInvariant(words[i][0]) +
                          (words[i].Length > 1 ? words[i][1..].ToLowerInvariant() : "");
            }
        }
        return string.Join(" ", words);
    }
}