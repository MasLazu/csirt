using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace MeUi.Infrastructure.Data.Seeders;

public class PageTenantPermissionSeeder
{
    private readonly IRepository<Page> _pageRepo;
    private readonly IRepository<TenantPermission> _tenantPermissionRepo; // now correctly using TenantPermission
    private readonly IRepository<PageTenantPermission> _pageTenantPermissionRepo;
    private readonly ILogger<PageTenantPermissionSeeder> _logger;

    public PageTenantPermissionSeeder(
        IRepository<Page> pageRepo,
    IRepository<TenantPermission> tenantPermissionRepo,
        IRepository<PageTenantPermission> pageTenantPermissionRepo,
        ILogger<PageTenantPermissionSeeder> logger)
    {
        _pageRepo = pageRepo;
        _tenantPermissionRepo = tenantPermissionRepo;
        _pageTenantPermissionRepo = pageTenantPermissionRepo;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken ct = default)
    {
        var pageTenantPermissionMap = new Dictionary<string, string[]>
        {
            { "2.1.1", new[] { "READ:THREAT_ANALYTICS" } },
            { "2.2.1", new[] { "READ:THREAT_ANALYTICS" } },
            { "2.2.2", new[] { "READ:THREAT_ANALYTICS" } },
            { "2.2.3", new[] { "READ:THREAT_ANALYTICS" } },
            { "2.2.4", new[] { "READ:THREAT_ANALYTICS" } },
            { "2.3.1", new[] { "READ:USER" } },
            { "2.3.2", new[] { "READ:ROLE" } },
            { "2.3.3", new[] { "READ:PERMISSION" } },
            { "2.4.1", new[] { "READ:TENANT" } },
            { "2.4.2", new[] { "READ:TENANT_ROLE" } },
            { "2.4.3", new[] { "READ:TENANT_USER" } },
            { "2.4.4", new[] { "READ:COUNTRY" } },
            { "2.5.1", new[] { "READ:PROTOCOL" } },
            { "2.5.2", new[] { "READ:ASN_REGISTRY" } },
            { "2.5.3", new[] { "READ:MALWARE_FAMILY" } },
            { "2.5.4", new[] { "READ:LOGIN_METHOD" } },
            { "2.5.5", new[] { "READ:PAGES" } },
        };

        var pages = (await _pageRepo.GetAllAsync(ct)).ToDictionary(p => p.Code, p => p.Id);
        IEnumerable<TenantPermission> tenantPermissions = await _tenantPermissionRepo.GetAllAsync(ct);
        var permLookup = tenantPermissions.ToDictionary(p => $"{p.ActionCode}:{p.ResourceCode}", p => p.Id);

        foreach (KeyValuePair<string, string[]> kvp in pageTenantPermissionMap)
        {
            string pageCode = kvp.Key;
            string[] permissionCodes = kvp.Value;
            if (!pages.TryGetValue(pageCode, out Guid pageId))
            {
                _logger.LogWarning("Tenant page code {PageCode} not found for tenant permission mapping", pageCode);
                continue;
            }
            foreach (string permissionCode in permissionCodes)
            {
                if (!permLookup.TryGetValue(permissionCode, out Guid tenantPermId))
                {
                    _logger.LogWarning("Tenant permission {PermissionCode} not found when mapping to page {PageCode}", permissionCode, pageCode);
                    continue;
                }
                bool exists = await _pageTenantPermissionRepo.FirstOrDefaultAsync(pp => pp.PageId == pageId && pp.TenantPermissionId == tenantPermId, ct) != null;
                if (!exists)
                {
                    await _pageTenantPermissionRepo.AddAsync(new PageTenantPermission { PageId = pageId, TenantPermissionId = tenantPermId }, ct);
                    _logger.LogInformation("Linked tenant page {PageCode} -> tenant permission {PermissionCode}", pageCode, permissionCode);
                }
            }
        }
    }
}
