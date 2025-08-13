using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace MeUi.Infrastructure.Data.Seeders;

public class PageSeeder
{
    private readonly IRepository<Page> _pageRepository;
    private readonly IRepository<PageGroup> _pageGroupRepository;
    private readonly ILogger<PageSeeder> _logger;

    public PageSeeder(IRepository<Page> pageRepository, IRepository<PageGroup> pageGroupRepository, ILogger<PageSeeder> logger)
    {
        _pageRepository = pageRepository;
        _pageGroupRepository = pageGroupRepository;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken ct = default)
    {
        // Fetch groups (new + legacy ones kept for backward compatibility)
        var groupCodes = new[]
        {
            "DASHBOARD","THREAT_ANALYTICS","THREAT_EVENTS","THREAT_INTELLIGENCE","NETWORK","GEO",
            "TENANT_ADMIN","USER_ADMIN","AUTHORIZATION","SETTINGS","TENANT_ANALYTICS","TENANT_DATA","TENANT_SECURITY",
            // legacy
            "USER_MANAGEMENT","SYSTEM","REPORTS"
        };

        var groupMap = new Dictionary<string, Guid?>();
        foreach (var code in groupCodes)
        {
            var g = await _pageGroupRepository.FirstOrDefaultAsync(x => x.Code == code, ct);
            groupMap[code] = g?.Id;
        }

        // Define hierarchical pages (ParentCode optional)
    var pages = new[]
        {
            // Global Home (no page group – root level)
            Page("DASHBOARD_HOME","Dashboard","/dashboard",null),
            // Tenant Home (no page group – root level within tenant workspace)
            Page("TENANT_HOME","Tenant Home","/tenants/{tenantId}",null),

            // Global Threat Analytics
            Page("TA_OVERVIEW","Overview","/threat-analytics/overview","THREAT_ANALYTICS"),
            Page("TA_SUMMARY","Summary","/threat-analytics/summary","THREAT_ANALYTICS"),
            Page("TA_TIMELINE","Timeline","/threat-analytics/timeline","THREAT_ANALYTICS"),
            Page("TA_TIMELINE_COMPARATIVE","Comparative Timeline","/threat-analytics/timeline/comparative","THREAT_ANALYTICS","TA_TIMELINE"),
            Page("TA_TIMELINE_CATEGORY","Category Timeline","/threat-analytics/categories/timeline","THREAT_ANALYTICS","TA_TIMELINE"),
            Page("TA_TIMELINE_MALWARE","Malware Family Timeline","/threat-analytics/malware-families/timeline","THREAT_ANALYTICS","TA_TIMELINE"),
            Page("TA_PROTOCOL_DISTRIBUTION","Protocol Distribution","/threat-analytics/protocols/distribution","THREAT_ANALYTICS"),
            Page("TA_GEO_HEATMAP","Geo Heatmap","/threat-analytics/geo/heatmap","THREAT_ANALYTICS"),
            Page("TA_TOP_ASNS","Top ASNs","/threat-analytics/asns/top","THREAT_ANALYTICS"),
            Page("TA_TOP_SOURCE_COUNTRIES","Top Source Countries","/threat-analytics/countries/source/top","THREAT_ANALYTICS"),

            // Threat Events
            Page("TE_LIST","Threat Events","/threat-events","THREAT_EVENTS"),
            Page("TE_DETAIL","Threat Event Detail","/threat-events/{id}","THREAT_EVENTS","TE_LIST"),

            // Threat Intelligence
            Page("TI_MALWARE_FAMILIES","Malware Families","/threat-intel/malware-families","THREAT_INTELLIGENCE"),
            Page("TI_MALWARE_FAMILY_DETAIL","Malware Family Detail","/threat-intel/malware-families/{id}","THREAT_INTELLIGENCE","TI_MALWARE_FAMILIES"),

            // Network
            Page("NET_PROTOCOLS","Protocols","/network/protocols","NETWORK"),
            Page("NET_PROTOCOL_DETAIL","Protocol Detail","/network/protocols/{id}","NETWORK","NET_PROTOCOLS"),
            Page("NET_ASN_REGISTRIES","ASN Registries","/network/asn-registries","NETWORK"),
            Page("NET_ASN_REGISTRY_DETAIL","ASN Registry Detail","/network/asn-registries/{id}","NETWORK","NET_ASN_REGISTRIES"),

            // Geography
            Page("GEO_COUNTRIES","Countries","/geo/countries","GEO"),
            Page("GEO_COUNTRY_DETAIL","Country Detail","/geo/countries/{id}","GEO","GEO_COUNTRIES"),

            // Tenant Administration (global view)
            Page("TEN_TENANTS","Tenants","/tenants","TENANT_ADMIN"),
            Page("TEN_TENANT_DETAIL","Tenant Detail","/tenants/{id}","TENANT_ADMIN","TEN_TENANTS"),
            Page("TEN_TENANT_ASNS","Tenant ASN Assignments","/tenants/{id}/asn-registries","TENANT_ADMIN","TEN_TENANT_DETAIL"),
            Page("TEN_TENANT_USERS","Tenant Users","/tenants/{id}/users","TENANT_ADMIN","TEN_TENANT_DETAIL"),
            Page("TEN_TENANT_ROLES","Tenant Roles","/tenants/{id}/roles","TENANT_ADMIN","TEN_TENANT_DETAIL"),
            Page("TEN_TENANT_ANALYTICS_PORTAL","Tenant Analytics Hub","/tenants/{id}/analytics","TENANT_ADMIN","TEN_TENANT_DETAIL"),

            // User & Roles (global)
            Page("UR_USERS","Users","/users","USER_ADMIN"),
            Page("UR_USER_DETAIL","User Detail","/users/{id}","USER_ADMIN","UR_USERS"),
            Page("UR_ROLES","Roles","/roles","USER_ADMIN"),
            Page("UR_ROLE_DETAIL","Role Detail","/roles/{id}","USER_ADMIN","UR_ROLES"),

            // Authorization Model
            Page("AUTH_PAGE_GROUPS","Page Groups","/auth/page-groups","AUTHORIZATION"),
            Page("AUTH_PAGES","Pages","/auth/pages","AUTHORIZATION"),
            Page("AUTH_PERMISSIONS","Permissions","/auth/permissions","AUTHORIZATION"),
            Page("AUTH_RESOURCES","Resources","/auth/resources","AUTHORIZATION"),
            Page("AUTH_ACTIONS","Actions","/auth/actions","AUTHORIZATION"),
            Page("AUTH_USER_ACCESS","User Accessible Pages","/auth/user-access","AUTHORIZATION"),

            // Settings
            Page("SET_LOGIN_METHODS","Login Methods","/settings/login-methods","SETTINGS"),
            Page("SET_SYSTEM","System Settings","/settings/system","SETTINGS"),

            // Tenant contextual analytics (inside tenant workspace navigation) - mirrors global analytics
            Page("TTA_OVERVIEW","Tenant Overview","/tenants/{tenantId}/analytics/overview","TENANT_ANALYTICS"),
            Page("TTA_SUMMARY","Tenant Summary","/tenants/{tenantId}/analytics/summary","TENANT_ANALYTICS"),
            Page("TTA_TIMELINE","Tenant Timeline","/tenants/{tenantId}/analytics/timeline","TENANT_ANALYTICS"),
            Page("TTA_TIMELINE_COMPARATIVE","Tenant Comparative Timeline","/tenants/{tenantId}/analytics/timeline/comparative","TENANT_ANALYTICS","TTA_TIMELINE"),
            Page("TTA_TIMELINE_CATEGORY","Tenant Category Timeline","/tenants/{tenantId}/analytics/categories/timeline","TENANT_ANALYTICS","TTA_TIMELINE"),
            Page("TTA_TIMELINE_MALWARE","Tenant Malware Family Timeline","/tenants/{tenantId}/analytics/malware-families/timeline","TENANT_ANALYTICS","TTA_TIMELINE"),
            Page("TTA_PROTOCOL_DISTRIBUTION","Tenant Protocol Distribution","/tenants/{tenantId}/analytics/protocols/distribution","TENANT_ANALYTICS"),
            Page("TTA_GEO_HEATMAP","Tenant Geo Heatmap","/tenants/{tenantId}/analytics/geo/heatmap","TENANT_ANALYTICS"),
            Page("TTA_TOP_ASNS","Tenant Top ASNs","/tenants/{tenantId}/analytics/asns/top","TENANT_ANALYTICS"),
            Page("TTA_TOP_SOURCE_COUNTRIES","Tenant Top Source Countries","/tenants/{tenantId}/analytics/countries/source/top","TENANT_ANALYTICS"),

            // Tenant data section
            Page("TD_THREAT_EVENTS","Tenant Threat Events","/tenants/{tenantId}/threat-events","TENANT_DATA"),
            Page("TD_THREAT_EVENT_DETAIL","Tenant Threat Event Detail","/tenants/{tenantId}/threat-events/{id}","TENANT_DATA","TD_THREAT_EVENTS"),
            Page("TD_ASN_ASSIGNMENTS","Tenant ASN Registries","/tenants/{tenantId}/asn-registries","TENANT_DATA"),
            Page("TD_ASN_ASSIGNMENT_DETAIL","Tenant ASN Registry Detail","/tenants/{tenantId}/asn-registries/{id}","TENANT_DATA","TD_ASN_ASSIGNMENTS"),

            // Tenant security section
            Page("TS_USERS","Tenant Users","/tenants/{tenantId}/users","TENANT_SECURITY"),
            Page("TS_USER_DETAIL","Tenant User Detail","/tenants/{tenantId}/users/{id}","TENANT_SECURITY","TS_USERS"),
            Page("TS_ROLES","Tenant Roles","/tenants/{tenantId}/roles","TENANT_SECURITY"),
            Page("TS_ROLE_DETAIL","Tenant Role Detail","/tenants/{tenantId}/roles/{id}","TENANT_SECURITY","TS_ROLES"),
            Page("TS_USER_ROLES","Tenant User Roles Assignment","/tenants/{tenantId}/users/{id}/roles","TENANT_SECURITY","TS_USER_DETAIL")
        };

        // Build lookup for inserted pages to resolve ParentId after creation if needed; simple approach: insert parent-first ordering already satisfied above.
        foreach (var p in pages)
        {
            Page? existing = await _pageRepository.FirstOrDefaultAsync(x => x.Code == p.Code, ct);
            if (existing == null)
            {
                await _pageRepository.AddAsync(new Page
                {
                    Code = p.Code,
                    Name = p.Name,
                    Path = p.Path,
                    PageGroupId = p.GroupCode == null ? null : (groupMap.TryGetValue(p.GroupCode, out var gid) ? gid : null),
                    ParentId = p.ParentCode == null ? null : (await _pageRepository.FirstOrDefaultAsync(x => x.Code == p.ParentCode, ct))?.Id
                }, ct);
                _logger.LogInformation("Seeded page: {Code}", p.Code);
            }
        }
    }

    private static (string Code, string Name, string Path, string? GroupCode, string? ParentCode) Page(string code, string name, string path, string? groupCode, string? parentCode = null)
        => (code, name, path, groupCode, parentCode);
}
