using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace MeUi.Infrastructure.Data.Seeders;

public class PageGroupSeeder
{
    private readonly IRepository<PageGroup> _pageGroupRepository;
    private readonly ILogger<PageGroupSeeder> _logger;

    public PageGroupSeeder(IRepository<PageGroup> pageGroupRepository, ILogger<PageGroupSeeder> logger)
    {
        _pageGroupRepository = pageGroupRepository;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken ct = default)
    {
        // NOTE: Previous seed introduced legacy groups (USER_MANAGEMENT, SYSTEM, REPORTS). We do not delete them here.
        // New normalized navigation groups (global + tenant-context) based on refined IA design.
        var pageGroups = new[]
        {
            new { Code = "DASHBOARD",          Name = "Dashboard",             Icon = "dashboard" },
            new { Code = "THREAT_ANALYTICS",   Name = "Threat Analytics",      Icon = "activity" },
            new { Code = "THREAT_EVENTS",      Name = "Threat Events",         Icon = "pulse" },
            new { Code = "THREAT_INTELLIGENCE",Name = "Threat Intelligence",   Icon = "shield-virus" },
            new { Code = "NETWORK",            Name = "Network Intelligence",  Icon = "network" },
            new { Code = "GEO",                Name = "Geography",             Icon = "globe" },
            new { Code = "TENANT_ADMIN",       Name = "Tenant Administration", Icon = "building" },
            new { Code = "USER_ADMIN",         Name = "Users & Roles",         Icon = "users" },
            new { Code = "AUTHORIZATION",      Name = "Authorization Model",   Icon = "key" },
            new { Code = "SETTINGS",           Name = "Settings",              Icon = "settings" },
            // Tenant contextual groupings (optional in UI, kept for segmentation)
            new { Code = "TENANT_ANALYTICS",   Name = "Tenant Analytics",      Icon = "activity" },
            new { Code = "TENANT_DATA",        Name = "Tenant Data",           Icon = "database" },
            new { Code = "TENANT_SECURITY",    Name = "Tenant Security",       Icon = "users-cog" }
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
}
