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
        var pageGroups = new[]
        {
            // Global sidebar (1)
            new { Code = "1.1", Name = "Overview", Icon = "home" },
            new { Code = "1.2", Name = "Threat Intelligence", Icon = "shield-virus" },
            new { Code = "1.3", Name = "Users & Access", Icon = "users" },
            new { Code = "1.4", Name = "Tenants", Icon = "building" },
            new { Code = "1.5", Name = "Resources", Icon = "database" },
            new { Code = "1.6", Name = "Settings & Audit", Icon = "settings" },

            // Tenant sidebar (2)
            new { Code = "2.1", Name = "Overview", Icon = "home" },
            new { Code = "2.2", Name = "Threat Intelligence", Icon = "shield-virus" },
            new { Code = "2.3", Name = "Users & Access", Icon = "users" },
            new { Code = "2.4", Name = "Resources", Icon = "database" },
            new { Code = "2.5", Name = "Settings & Audit", Icon = "settings" }
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
