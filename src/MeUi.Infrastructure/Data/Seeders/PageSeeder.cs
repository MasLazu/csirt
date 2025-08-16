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
        string[] groupCodes = new[]
        {
            // Global sidebar (1)
            "1.1", // Dashboard
            "1.2", // Threat Intelligence
            "1.3", // Users & Access
            "1.4", // Tenants
            "1.5", // Resources
            "1.6", // Settings & Audit

            // Tenant sidebar (2) - same as global, but without tenant management
            "2.1", // Dashboard
            "2.2", // Threat Intelligence
            "2.3", // Users & Access
            "2.4", // Resources
            "2.5", // Settings & Audit
        };

        var pages = new (string Code, string Name, string Path, string? GroupCode, string? ParentCode)[]
        {
            // Global Sidebar (1)
            Page("1.1.1","Overview","/overview","1.1"),

            Page("1.2.1","Timeline & Trends","/threat-intelligence/timeline-trends","1.2"),
            Page("1.2.2","Geographic Insights","/threat-intelligence/geographic-insights","1.2"),
            Page("1.2.3","Entity Analytics","/threat-intelligence/entity-analytics","1.2"),
            Page("1.2.4","Deep Dive","/threat-intelligence/deep-dive","1.2"),

            Page("1.3.1","Users","/users-&-access/users","1.3"),
            Page("1.3.2","Roles","/users-&-access/roles","1.3"),
            Page("1.3.3","Permissions","/users-&-access/permissions","1.3"),

            Page("1.4.1","Tenant Directory","/tenants/tenant-directory","1.4"),
            Page("1.4.2","Tenant Roles","/tenants/tenant-roles","1.4"),
            Page("1.4.3","Tenant Users","/tenants/tenant-users","1.4"),

            Page("1.5.1","Countries","/resources/countries","1.5"),
            Page("1.5.2","Protocols","/resources/protocols","1.5"),
            Page("1.5.3","ASN Registries","/resources/asn-registries","1.5"),
            Page("1.5.4","Malware Families","/resources/malware-families","1.5"),

            Page("1.6.1","System Settings","/settings-audit/system-settings","1.6"),
            Page("1.6.2","Audit Logs","/settings-audit/audit-logs","1.6"),

            // Tenant Sidebar (2)
            Page("2.1.1","Overview","/tenant/{tenantId}/overview","2.1"),

            Page("2.2.1","Timeline & Trends","/tenant/{tenantId}/threat-intelligence/timeline-trends","2.2"),
            Page("2.2.2","Geographic Insights","/tenant/{tenantId}/threat-intelligence/geographic-insights","2.2"),
            Page("2.2.3","Entity Analytics","/tenant/{tenantId}/threat-intelligence/entity-analytics","2.2"),
            Page("2.2.4","Deep Dive","/tenant/{tenantId}/threat-intelligence/deep-dive","2.2"),

            Page("2.3.1","Users","/tenant/{tenantId}/users-&-access/users","2.3"),
            Page("2.3.2","Roles","/tenant/{tenantId}/users-&-access/roles","2.3"),
            Page("2.3.3","Permissions","/tenant/{tenantId}/users-&-access/permissions","2.3"),

            Page("2.4.1","Countries","/tenant/{tenantId}/resources/countries","2.4"),
            Page("2.4.2","Protocols","/tenant/{tenantId}/resources/protocols","2.4"),
            Page("2.4.3","ASN Registries","/tenant/{tenantId}/resources/asn-registries","2.4"),
            Page("2.4.4","Malware Families","/tenant/{tenantId}/resources/malware-families","2.4"),

            Page("2.5.1","System Settings","/tenant/{tenantId}/settings-audit/system-settings","2.5"),
            Page("2.5.2","Audit Logs","/tenant/{tenantId}/settings-audit/audit-logs","2.5"),
        };
        // Build groupCode to PageGroup Id map
        var groupMap = new Dictionary<string, Guid?>();
        foreach (string groupCode in groupCodes)
        {
            PageGroup? group = await _pageGroupRepository.FirstOrDefaultAsync(x => x.Code == groupCode, ct);
            groupMap[groupCode] = group?.Id;
        }

        // Build lookup for inserted pages to resolve ParentId after creation if needed; simple approach: insert parent-first ordering already satisfied above.
        foreach ((string code, string name, string path, string? groupCode, string? parentCode) in pages)
        {
            Page? existing = await _pageRepository.FirstOrDefaultAsync(x => x.Code == code, ct);
            if (existing == null)
            {
                Guid? pageGroupId = groupCode == null ? null : (groupMap.TryGetValue(groupCode, out Guid? gid) ? gid : null);
                Guid? parentId = parentCode == null ? null : (await _pageRepository.FirstOrDefaultAsync(x => x.Code == parentCode, ct))?.Id;
                await _pageRepository.AddAsync(new Page
                {
                    Code = code,
                    Name = name,
                    Path = path,
                    PageGroupId = pageGroupId,
                    ParentId = parentId
                }, ct);
                _logger.LogInformation("Seeded page: {Code}", code);
            }
        }
    }

    private static (string Code, string Name, string Path, string? GroupCode, string? ParentCode) Page(string code, string name, string path, string? groupCode, string? parentCode = null)
        => (code, name, path, groupCode, parentCode);
}
