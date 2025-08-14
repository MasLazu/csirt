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
        var mappings = new (string PageCode, string TenantPermissionCode)[]
        {
            ("TENANT_HOME","READ:THREAT_ANALYTICS"),
            ("TTA_OVERVIEW","READ:THREAT_ANALYTICS"),("TTA_SUMMARY","READ:THREAT_ANALYTICS"),("TTA_TIMELINE","READ:THREAT_ANALYTICS"),
            ("TTA_TIMELINE_COMPARATIVE","READ:THREAT_ANALYTICS"),("TTA_TIMELINE_CATEGORY","READ:THREAT_ANALYTICS"),("TTA_TIMELINE_MALWARE","READ:THREAT_ANALYTICS"),
            ("TTA_PROTOCOL_DISTRIBUTION","READ:THREAT_ANALYTICS"),("TTA_GEO_HEATMAP","READ:THREAT_ANALYTICS"),("TTA_TOP_ASNS","READ:THREAT_ANALYTICS"),("TTA_TOP_SOURCE_COUNTRIES","READ:THREAT_ANALYTICS"),
            ("TD_THREAT_EVENTS","READ:THREAT_EVENT"),
            ("TD_ASN_ASSIGNMENTS","READ:TENANT_ASN"),
            ("TS_USERS","READ:TENANT_USER"),
            ("TS_ROLES","READ:TENANT_ROLE")
        };

        var pages = (await _pageRepo.GetAllAsync(ct)).ToDictionary(p => p.Code, p => p.Id);
        var tenantPermissions = await _tenantPermissionRepo.GetAllAsync(ct);
        var permLookup = tenantPermissions.ToDictionary(p => $"{p.ActionCode}:{p.ResourceCode}", p => p.Id);

        foreach (var m in mappings)
        {
            if (!pages.TryGetValue(m.PageCode, out var pageId))
            {
                _logger.LogWarning("Tenant page code {PageCode} not found for tenant permission mapping {PermissionCode}", m.PageCode, m.TenantPermissionCode);
                continue;
            }
            if (!permLookup.TryGetValue(m.TenantPermissionCode, out var tenantPermId))
            {
                _logger.LogWarning("Tenant permission {PermissionCode} not found when mapping to page {PageCode}", m.TenantPermissionCode, m.PageCode);
                continue;
            }
            bool exists = await _pageTenantPermissionRepo.FirstOrDefaultAsync(pp => pp.PageId == pageId && pp.TenantPermissionId == tenantPermId, ct) != null;
            if (!exists)
            {
                await _pageTenantPermissionRepo.AddAsync(new PageTenantPermission { PageId = pageId, TenantPermissionId = tenantPermId }, ct);
                _logger.LogInformation("Linked tenant page {PageCode} -> tenant permission {PermissionCode}", m.PageCode, m.TenantPermissionCode);
            }
        }
    }
}
