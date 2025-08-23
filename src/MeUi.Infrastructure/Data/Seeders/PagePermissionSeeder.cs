using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace MeUi.Infrastructure.Data.Seeders;

public class PagePermissionSeeder
{
    private readonly IRepository<Page> _pageRepo;
    private readonly IRepository<Permission> _permissionRepo;
    private readonly IRepository<PagePermission> _pagePermissionRepo;
    private readonly ILogger<PagePermissionSeeder> _logger;

    public PagePermissionSeeder(
        IRepository<Page> pageRepo,
        IRepository<Permission> permissionRepo,
        IRepository<PagePermission> pagePermissionRepo,
        ILogger<PagePermissionSeeder> logger)
    {
        _pageRepo = pageRepo;
        _permissionRepo = permissionRepo;
        _pagePermissionRepo = pagePermissionRepo;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken ct = default)
    {
        var pagePermissionMap = new Dictionary<string, string[]>
            {
            { "1.1.1", new[] { "READ:THREAT_INTELLIGENT_OVERVIEW" } },
            { "1.2.1", new[] { "READ:THREAT_TEMPORAL" } },
            { "1.2.2", new[] { "READ:THREAT_GEOGRAPHIC" } },
            { "1.2.3", new[] { "READ:THREAT_ACTORS" } },
            { "1.2.4", new[] { "READ:THREAT_COMPLIANCE" } },
            { "1.2.5", new[] { "READ:THREAT_NETWORK" } },
            { "1.2.6", new[] { "READ:THREAT_INCIDENT" } },
            { "1.2.7", new[] { "READ:THREAT_MALWARE" } },
            { "1.3.1", new[] { "READ:USER" } },
            { "1.3.2", new[] { "READ:ROLE" } },
            { "1.3.3", new[] { "READ:PERMISSION" } },
            { "1.4.1", new[] { "READ:TENANT" } },
            { "1.4.2", new[] { "READ:TENANT_ROLE" } },
            { "1.4.3", new[] { "READ:TENANT_USER" } },
            { "1.5.1", new[] { "READ:COUNTRY" } },
            { "1.5.2", new[] { "READ:PROTOCOL" } },
            { "1.5.3", new[] { "READ:ASN_REGISTRY" } },
            { "1.5.4", new[] { "READ:MALWARE_FAMILY" } },
            { "1.6.1", new[] { "READ:LOGIN_METHOD" } },
            { "1.6.2", new[] { "READ:PAGES" } },
        };

        var pages = (await _pageRepo.GetAllAsync(ct)).ToDictionary(p => p.Code, p => p.Id);
        IEnumerable<Permission> permissions = await _permissionRepo.GetAllAsync(ct);
        var permLookup = permissions.ToDictionary(p => $"{p.ActionCode}:{p.ResourceCode}", p => p.Id);

        foreach (KeyValuePair<string, string[]> kvp in pagePermissionMap)
        {
            string pageCode = kvp.Key;
            string[] permissionCodes = kvp.Value;
            if (!pages.TryGetValue(pageCode, out Guid pageId))
            {
                _logger.LogWarning("Page code {PageCode} not found for permission mapping", pageCode);
                continue;
            }
            foreach (string permissionCode in permissionCodes)
            {
                if (!permLookup.TryGetValue(permissionCode, out Guid permId))
                {
                    _logger.LogWarning("Permission {PermissionCode} not found when mapping to page {PageCode}", permissionCode, pageCode);
                    continue;
                }
                bool exists = await _pagePermissionRepo.FirstOrDefaultAsync(pp => pp.PageId == pageId && pp.PermissionId == permId, ct) != null;
                if (!exists)
                {
                    await _pagePermissionRepo.AddAsync(new PagePermission { PageId = pageId, PermissionId = permId }, ct);
                    _logger.LogInformation("Linked page {PageCode} -> permission {PermissionCode}", pageCode, permissionCode);
                }
            }
        }
    }
}
