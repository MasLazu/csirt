using System.Reflection;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace MeUi.Infrastructure.Data.Seeders;

public class TenantPermissionSeeder
{
    private readonly IRepository<Resource> _resourceRepository;
    private readonly IRepository<Domain.Entities.Action> _actionRepository;
    private readonly IRepository<TenantPermission> _tenantPermissionRepository;
    private readonly ILogger<TenantPermissionSeeder> _logger;

    public TenantPermissionSeeder(
        IRepository<Resource> resourceRepository,
        IRepository<Domain.Entities.Action> actionRepository,
    IRepository<TenantPermission> tenantPermissionRepository,
        ILogger<TenantPermissionSeeder> logger)
    {
        _resourceRepository = resourceRepository;
        _actionRepository = actionRepository;
        _tenantPermissionRepository = tenantPermissionRepository;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken ct = default)
    {
        _logger.LogInformation("Starting tenant permission seeding from ITenantPermissionProvider implementations");

        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
            .ToList();

        List<Type> tenantPermissionProviders = new();

        foreach (Assembly assembly in assemblies)
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

        HashSet<string> discoveredTenantPermissions = new();

        string[] nameCandidates = ["TenantPermission", "Permission"]; // allow both naming styles

        foreach (Type providerType in tenantPermissionProviders)
        {
            try
            {
                bool foundAny = false;
                foreach (string name in nameCandidates)
                {
                    PropertyInfo? prop = providerType.GetProperty(name, BindingFlags.Public | BindingFlags.Static);
                    if (prop != null && prop.PropertyType == typeof(string))
                    {
                        if (prop.GetValue(null) is string value && !string.IsNullOrWhiteSpace(value))
                        {
                            if (discoveredTenantPermissions.Add(value))
                            {
                                _logger.LogDebug("Discovered tenant permission (property {Prop}): {Permission} from {ProviderType}", name, value, providerType.Name);
                            }
                            foundAny = true;
                        }
                    }
                }

                foreach (string name in nameCandidates)
                {
                    MethodInfo? method = providerType.GetMethod(name, BindingFlags.Public | BindingFlags.Static, Array.Empty<Type>());
                    if (method != null && method.ReturnType == typeof(string) && method.GetParameters().Length == 0)
                    {
                        if (method.Invoke(null, null) is string value && !string.IsNullOrWhiteSpace(value))
                        {
                            if (discoveredTenantPermissions.Add(value))
                            {
                                _logger.LogDebug("Discovered tenant permission (method {Method}): {Permission} from {ProviderType}", name, value, providerType.Name);
                            }
                            foundAny = true;
                        }
                    }
                }

                if (!foundAny)
                {
                    _logger.LogTrace("No tenant permission static member found on {ProviderType}", providerType.Name);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting tenant permission from {ProviderType}", providerType.Name);
            }
        }

        _logger.LogInformation("Discovered {Count} unique tenant permissions", discoveredTenantPermissions.Count);

        List<(string ActionCode, string ResourceCode)> parsedTenantPermissions = new();
        HashSet<string> uniqueTenantResources = new();
        HashSet<string> uniqueTenantActions = new();

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

        foreach (string resourceCode in uniqueTenantResources)
        {
            Resource? existing = await _resourceRepository.FirstOrDefaultAsync(r => r.Code == resourceCode, ct);
            if (existing == null)
            {
                Resource resource = new()
                {
                    Code = resourceCode,
                    Name = ToTitleCase(resourceCode.Replace("_", " ")),
                    Description = $"Resource for {resourceCode.ToLowerInvariant()}"
                };
                await _resourceRepository.AddAsync(resource, ct);
                _logger.LogInformation("Created tenant resource: {ResourceCode}", resourceCode);
            }
        }

        foreach (string actionCode in uniqueTenantActions)
        {
            Domain.Entities.Action? existing = await _actionRepository.FirstOrDefaultAsync(a => a.Code == actionCode, ct);
            if (existing == null)
            {
                Domain.Entities.Action action = new()
                {
                    Code = actionCode,
                    Name = ToTitleCase(actionCode.Replace("_", " ")),
                    Description = $"Action for {actionCode.ToLowerInvariant()}"
                };
                await _actionRepository.AddAsync(action, ct);
                _logger.LogInformation("Created tenant action: {ActionCode}", actionCode);
            }
        }

        foreach ((string actionCode, string resourceCode) in parsedTenantPermissions)
        {
            TenantPermission? existing = await _tenantPermissionRepository.FirstOrDefaultAsync(
                p => p.ResourceCode == resourceCode && p.ActionCode == actionCode, ct);

            if (existing == null)
            {
                TenantPermission tenantPermission = new()
                {
                    ResourceCode = resourceCode,
                    ActionCode = actionCode
                };
                await _tenantPermissionRepository.AddAsync(tenantPermission, ct);
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
