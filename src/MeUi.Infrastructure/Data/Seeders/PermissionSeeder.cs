using System.Reflection;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace MeUi.Infrastructure.Data.Seeders;

public class PermissionSeeder
{
    private readonly IRepository<Resource> _resourceRepository;
    private readonly IRepository<Domain.Entities.Action> _actionRepository;
    private readonly IRepository<Permission> _permissionRepository;
    private readonly ILogger<PermissionSeeder> _logger;

    public PermissionSeeder(
        IRepository<Resource> resourceRepository,
        IRepository<Domain.Entities.Action> actionRepository,
        IRepository<Permission> permissionRepository,
        ILogger<PermissionSeeder> logger)
    {
        _resourceRepository = resourceRepository;
        _actionRepository = actionRepository;
        _permissionRepository = permissionRepository;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken ct = default)
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
