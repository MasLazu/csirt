using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;
using MeUi.Infrastructure.Data;
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
    private readonly IRepository<UserLoginMethod> _userLoginMethodRepository;
    private readonly IRepository<Password> _passwordRepository;
    private readonly IRepository<UserRole> _userRoleRepository;
    private readonly ApplicationDbContext _context;
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
        IRepository<Tenant> tenantRepository,
        IRepository<TenantUser> tenantUserRepository,
        ApplicationDbContext context,
        IRepository<TenantUserLoginMethod> tenantUserLoginMethodRepository,
        IRepository<TenantUserPassword> tenantUserPasswordRepository,
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
        _userLoginMethodRepository = userLoginMethodRepository;
        _passwordRepository = passwordRepository;
        _userRoleRepository = userRoleRepository;
        _context = context;
        _unitOfWork = unitOfWork;
        _configuration = configuration;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken ct = default)
    {
        try
        {
            await SeedLoginMethodsAsync(ct);
            await _unitOfWork.SaveChangesAsync(ct);

            await SeedPermissionsAsync(ct);
            await _unitOfWork.SaveChangesAsync(ct);

            await SeedTenantPermissionsAsync(ct);
            await _unitOfWork.SaveChangesAsync(ct);

            await SeedSuperRolesAsync(ct);
            await _unitOfWork.SaveChangesAsync(ct);

            await SeedPageGroupsAsync(ct);
            await _unitOfWork.SaveChangesAsync(ct);

            await SeedPagesAsync(ct);
            await _unitOfWork.SaveChangesAsync(ct);

            await SeedSuperUserAsync(ct);
            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during database seeding");
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
            LoginMethod? existing = await _loginMethodRepository.FirstOrDefaultAsync(x => x.Code == method.Code, ct);
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
            Id = Guid.Parse("01989299-2c61-71a0-92b9-5a7700dd263e"),
            Name = "Super Administrator",
            Description = "Full system access"
        };

        Role? existing = await _roleRepository.FirstOrDefaultAsync(x => x.Id == role.Id, ct);
        if (existing == null)
        {
            await _roleRepository.AddAsync(role, ct);
            _logger.LogInformation("Seeded role: {Code}", role.Name);
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

        User? existingUser = await _userRepository.FirstOrDefaultAsync(x => x.Email == superUserEmail, ct);
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
            PageGroup? existing = await _pageGroupRepository.FirstOrDefaultAsync(x => x.Code == group.Code, ct);
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
        PageGroup? dashboardGroup = await _pageGroupRepository.FirstOrDefaultAsync(x => x.Code == "DASHBOARD", ct);
        PageGroup? userMgmtGroup = await _pageGroupRepository.FirstOrDefaultAsync(x => x.Code == "USER_MANAGEMENT", ct);
        PageGroup? authGroup = await _pageGroupRepository.FirstOrDefaultAsync(x => x.Code == "AUTHORIZATION", ct);
        PageGroup? systemGroup = await _pageGroupRepository.FirstOrDefaultAsync(x => x.Code == "SYSTEM", ct);
        PageGroup? reportsGroup = await _pageGroupRepository.FirstOrDefaultAsync(x => x.Code == "REPORTS", ct);

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
            Page? existing = await _pageRepository.FirstOrDefaultAsync(x => x.Code == page.Code, ct);
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

        foreach (Assembly? assembly in assemblies)
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

        foreach (Type providerType in permissionProviders)
        {
            try
            {
                MethodInfo? permissionMethod = providerType.GetMethod("Permission",
                    BindingFlags.Public | BindingFlags.Static);

                if (permissionMethod != null)
                {
                    string? permission = permissionMethod.Invoke(null, null) as string;
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

        foreach (string permissionString in discoveredPermissions)
        {
            string[] parts = permissionString.Split(':', 2);
            if (parts.Length != 2)
            {
                _logger.LogWarning("Invalid permission format: {Permission}. Expected format: 'action:resource'",
                    permissionString);
                continue;
            }

            string actionCode = parts[0].Trim().ToUpperInvariant();
            string resourceCode = parts[1].Trim().ToUpperInvariant();

            parsedPermissions.Add((actionCode, resourceCode));
            uniqueResources.Add(resourceCode);
            uniqueActions.Add(actionCode);
        }

        // Create resources first
        foreach (string resourceCode in uniqueResources)
        {
            Resource? existing = await _resourceRepository.FirstOrDefaultAsync(r => r.Code == resourceCode, ct);
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
        foreach (string actionCode in uniqueActions)
        {
            Domain.Entities.Action? existing = await _actionRepository.FirstOrDefaultAsync(a => a.Code == actionCode, ct);
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
        foreach ((string actionCode, string resourceCode) in parsedPermissions)
        {
            Permission? existing = await _permissionRepository.FirstOrDefaultAsync(
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

    private async Task SeedTenantPermissionsAsync(CancellationToken ct)
    {
        _logger.LogInformation("Starting tenant permission seeding from ITenantPermissionProvider implementations");

        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
            .ToList();

        var tenantPermissionProviders = new List<Type>();

        foreach (Assembly? assembly in assemblies)
        {
            try
            {
                var types = assembly.GetTypes()
                    .Where(t => t.IsClass && !t.IsAbstract &&
                               t.GetInterfaces().Any(i => i == typeof(ITenantPermissionProvider)))
                    .ToList();

                tenantPermissionProviders.AddRange(types);
            }
            catch (ReflectionTypeLoadException ex)
            {
                _logger.LogWarning("Could not load types from assembly {AssemblyName}: {Error}",
                    assembly.FullName, ex.Message);
                continue;
            }
        }

        _logger.LogInformation("Found {Count} tenant permission provider classes", tenantPermissionProviders.Count);

        var discoveredTenantPermissions = new HashSet<string>();

        foreach (Type providerType in tenantPermissionProviders)
        {
            try
            {
                PropertyInfo? permissionProperty = providerType.GetProperty("Permission",
                    BindingFlags.Public | BindingFlags.Static);

                if (permissionProperty != null)
                {
                    string? permission = permissionProperty.GetValue(null) as string;
                    if (!string.IsNullOrEmpty(permission))
                    {
                        discoveredTenantPermissions.Add(permission);
                        _logger.LogDebug("Discovered tenant permission: {Permission} from {ProviderType}",
                            permission, providerType.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting tenant permission from {ProviderType}", providerType.Name);
            }
        }

        _logger.LogInformation("Discovered {Count} unique tenant permissions", discoveredTenantPermissions.Count);

        // Parse and collect unique resources and actions for tenant permissions
        var parsedTenantPermissions = new List<(string ActionCode, string ResourceCode)>();
        var uniqueTenantResources = new HashSet<string>();
        var uniqueTenantActions = new HashSet<string>();

        foreach (string permissionString in discoveredTenantPermissions)
        {
            string[] parts = permissionString.Split(':', 2);
            if (parts.Length != 2)
            {
                _logger.LogWarning("Invalid tenant permission format: {Permission}. Expected format: 'action:resource'",
                    permissionString);
                continue;
            }

            string actionCode = parts[0].Trim().ToUpperInvariant();
            string resourceCode = parts[1].Trim().ToUpperInvariant();

            parsedTenantPermissions.Add((actionCode, resourceCode));
            uniqueTenantResources.Add(resourceCode);
            uniqueTenantActions.Add(actionCode);
        }

        // Create tenant resources first
        foreach (string resourceCode in uniqueTenantResources)
        {
            Resource? existing = await _resourceRepository.FirstOrDefaultAsync(r => r.Code == resourceCode, ct);
            if (existing == null)
            {
                var resource = new Resource
                {
                    Code = resourceCode,
                    Name = ToTitleCase(resourceCode.Replace("_", " ")),
                    Description = $"Resource for {resourceCode.ToLowerInvariant()}"
                };
                await _resourceRepository.AddAsync(resource, ct);
                _logger.LogInformation("Created tenant resource: {ResourceCode}", resourceCode);
            }
        }

        // Create tenant actions
        foreach (string actionCode in uniqueTenantActions)
        {
            Domain.Entities.Action? existing = await _actionRepository.FirstOrDefaultAsync(a => a.Code == actionCode, ct);
            if (existing == null)
            {
                var action = new Domain.Entities.Action
                {
                    Code = actionCode,
                    Name = ToTitleCase(actionCode.Replace("_", " ")),
                    Description = $"Action for {actionCode.ToLowerInvariant()}"
                };
                await _actionRepository.AddAsync(action, ct);
                _logger.LogInformation("Created tenant action: {ActionCode}", actionCode);
            }
        }

        // Create tenant permissions
        foreach ((string actionCode, string resourceCode) in parsedTenantPermissions)
        {
            Permission? existing = await _permissionRepository.FirstOrDefaultAsync(
                p => p.ResourceCode == resourceCode && p.ActionCode == actionCode, ct);

            if (existing == null)
            {
                var permission = new Permission
                {
                    ResourceCode = resourceCode,
                    ActionCode = actionCode
                };
                await _permissionRepository.AddAsync(permission, ct);
                _logger.LogInformation("Created tenant permission: {ActionCode}:{ResourceCode}", actionCode, resourceCode);
            }
        }

        _logger.LogInformation("Tenant permission seeding completed");
    }

    private static string ToTitleCase(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        string[] words = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
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