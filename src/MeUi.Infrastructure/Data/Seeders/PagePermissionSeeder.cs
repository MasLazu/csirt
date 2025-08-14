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
        var mappings = new (string PageCode, string PermissionCode)[]
        {
            ("DASHBOARD_HOME","READ:THREAT_ANALYTICS"),
            // Global analytics
            ("TA_OVERVIEW","READ:THREAT_ANALYTICS"),
            ("TA_SUMMARY","READ:THREAT_ANALYTICS"),
            ("TA_TIMELINE","READ:THREAT_ANALYTICS"),
            ("TA_TIMELINE_COMPARATIVE","READ:THREAT_ANALYTICS"),
            ("TA_TIMELINE_CATEGORY","READ:THREAT_ANALYTICS"),
            ("TA_TIMELINE_MALWARE","READ:THREAT_ANALYTICS"),
            ("TA_PROTOCOL_DISTRIBUTION","READ:THREAT_ANALYTICS"),
            ("TA_GEO_HEATMAP","READ:THREAT_ANALYTICS"),
            ("TA_TOP_ASNS","READ:THREAT_ANALYTICS"),
            ("TA_TOP_SOURCE_COUNTRIES","READ:THREAT_ANALYTICS"),
            // Threat events
            ("TE_LIST","READ:THREAT_EVENT"),("TE_DETAIL","READ:THREAT_EVENT"),
            // Threat intel
            ("TI_MALWARE_FAMILIES","READ:MALWARE_FAMILY"),("TI_MALWARE_FAMILY_DETAIL","READ:MALWARE_FAMILY"),
            // Network
            ("NET_PROTOCOLS","READ:PROTOCOL"),("NET_PROTOCOL_DETAIL","READ:PROTOCOL"),
            ("NET_ASN_REGISTRIES","READ:ASN_REGISTRY"),("NET_ASN_REGISTRY_DETAIL","READ:ASN_REGISTRY"),
            // Geography
            ("GEO_COUNTRIES","READ:COUNTRY"),("GEO_COUNTRY_DETAIL","READ:COUNTRY"),
            // Tenant admin global perspective
            ("TEN_TENANTS","READ:TENANT"),("TEN_TENANT_DETAIL","READ:TENANT"),
            ("TEN_TENANT_ASNS","READ:TENANT_ASN"),("TEN_TENANT_USERS","READ:TENANT_USER"),("TEN_TENANT_ROLES","READ:TENANT_ROLE"),
            ("TEN_TENANT_ANALYTICS_PORTAL","READ:THREAT_ANALYTICS"),
            // User & Roles global
            ("UR_USERS","READ:ROLE"),("UR_USER_DETAIL","READ:ROLE"),
            ("UR_ROLES","READ:ROLE"),("UR_ROLE_DETAIL","READ:ROLE"),
            // Authorization model
            ("AUTH_PAGE_GROUPS","READ:PAGE_GROUP"),("AUTH_PAGES","READ:PAGE"),("AUTH_PERMISSIONS","READ:ROLE"),
            ("AUTH_RESOURCES","READ:RESOURCE"),("AUTH_ACTIONS","READ:ACTION"),("AUTH_USER_ACCESS","READ:USER_ACCESSIBLE_PAGES"),
            // Settings
            ("SET_LOGIN_METHODS","READ:LOGIN_METHOD"),("SET_SYSTEM","READ:RESOURCE"),
            // Tenant context pages also get global override permission
            ("TENANT_HOME","READ:THREAT_ANALYTICS"),
            ("TTA_OVERVIEW","READ:THREAT_ANALYTICS"),("TTA_SUMMARY","READ:THREAT_ANALYTICS"),("TTA_TIMELINE","READ:THREAT_ANALYTICS"),
            ("TTA_TIMELINE_COMPARATIVE","READ:THREAT_ANALYTICS"),("TTA_TIMELINE_CATEGORY","READ:THREAT_ANALYTICS"),("TTA_TIMELINE_MALWARE","READ:THREAT_ANALYTICS"),
            ("TTA_PROTOCOL_DISTRIBUTION","READ:THREAT_ANALYTICS"),("TTA_GEO_HEATMAP","READ:THREAT_ANALYTICS"),("TTA_TOP_ASNS","READ:THREAT_ANALYTICS"),("TTA_TOP_SOURCE_COUNTRIES","READ:THREAT_ANALYTICS"),
            ("TD_THREAT_EVENTS","READ:THREAT_EVENT"),("TD_THREAT_EVENT_DETAIL","READ:THREAT_EVENT"),
            ("TD_ASN_ASSIGNMENTS","READ:TENANT_ASN"),("TD_ASN_ASSIGNMENT_DETAIL","READ:TENANT_ASN"),
            ("TS_USERS","READ:TENANT_USER"),("TS_USER_DETAIL","READ:TENANT_USER"),
            ("TS_ROLES","READ:TENANT_ROLE"),("TS_ROLE_DETAIL","READ:TENANT_ROLE"),("TS_USER_ROLES","READ:TENANT_USER_ROLES")
        };

        var pages = (await _pageRepo.GetAllAsync(ct)).ToDictionary(p => p.Code, p => p.Id);
        IEnumerable<Permission> permissions = await _permissionRepo.GetAllAsync(ct);
        var permLookup = permissions.ToDictionary(p => $"{p.ActionCode}:{p.ResourceCode}", p => p.Id);

        foreach (var m in mappings)
        {
            if (!pages.TryGetValue(m.PageCode, out Guid pageId))
            {
                _logger.LogWarning("Page code {PageCode} not found for permission mapping {PermissionCode}", m.PageCode, m.PermissionCode);
                continue;
            }
            if (!permLookup.TryGetValue(m.PermissionCode, out Guid permId))
            {
                _logger.LogWarning("Permission {PermissionCode} not found when mapping to page {PageCode}", m.PermissionCode, m.PageCode);
                continue;
            }
            bool exists = await _pagePermissionRepo.FirstOrDefaultAsync(pp => pp.PageId == pageId && pp.PermissionId == permId, ct) != null;
            if (!exists)
            {
                await _pagePermissionRepo.AddAsync(new PagePermission { PageId = pageId, PermissionId = permId }, ct);
                _logger.LogInformation("Linked page {PageCode} -> permission {PermissionCode}", m.PageCode, m.PermissionCode);
            }
        }
    }
}
