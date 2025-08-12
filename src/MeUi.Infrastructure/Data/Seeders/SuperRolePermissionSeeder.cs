using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace MeUi.Infrastructure.Data.Seeders;

public class SuperRolePermissionSeeder
{
    private readonly IRepository<Role> _roleRepository;
    private readonly IRepository<Permission> _permissionRepository;
    private readonly IRepository<RolePermission> _rolePermissionRepository;
    private readonly ILogger<SuperRolePermissionSeeder> _logger;

    public SuperRolePermissionSeeder(
        IRepository<Role> roleRepository,
        IRepository<Permission> permissionRepository,
        IRepository<RolePermission> rolePermissionRepository,
        ILogger<SuperRolePermissionSeeder> logger)
    {
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _rolePermissionRepository = rolePermissionRepository;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken ct = default)
    {
        var superRoleId = Guid.Parse("01989299-2c61-71a0-92b9-5a7700dd263e");

        Role? superRole = await _roleRepository.FirstOrDefaultAsync(r => r.Id == superRoleId, ct);
        if (superRole == null)
        {
            _logger.LogWarning("Super role not found when assigning permissions");
            return;
        }

        IEnumerable<Permission> allPermissions = await _permissionRepository.GetAllAsync(ct);
        IEnumerable<RolePermission> existingRolePermissions = await _rolePermissionRepository.FindAsync(rp => rp.RoleId == superRoleId, ct);
        var existingPermissionIds = existingRolePermissions.Select(rp => rp.PermissionId).ToHashSet();

        var toAdd = allPermissions
            .Where(p => !existingPermissionIds.Contains(p.Id))
            .Select(p => new RolePermission
            {
                RoleId = superRoleId,
                PermissionId = p.Id
            })
            .ToList();

        if (toAdd.Count == 0)
        {
            _logger.LogInformation("All permissions already assigned to super role");
            return;
        }

        await _rolePermissionRepository.AddRangeAsync(toAdd, ct);
        _logger.LogInformation("Assigned {Count} new permissions to super role", toAdd.Count);
    }
}
