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
            new { Code = "DASHBOARD", Name = "Dashboard", Icon = "dashboard" },
            new { Code = "USER_MANAGEMENT", Name = "User Management", Icon = "people" },
            new { Code = "AUTHORIZATION", Name = "Authorization", Icon = "security" },
            new { Code = "SYSTEM", Name = "System", Icon = "settings" },
            new { Code = "REPORTS", Name = "Reports", Icon = "analytics" }
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
