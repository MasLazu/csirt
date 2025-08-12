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
        PageGroup? dashboardGroup = await _pageGroupRepository.FirstOrDefaultAsync(x => x.Code == "DASHBOARD", ct);
        PageGroup? userMgmtGroup = await _pageGroupRepository.FirstOrDefaultAsync(x => x.Code == "USER_MANAGEMENT", ct);
        PageGroup? authGroup = await _pageGroupRepository.FirstOrDefaultAsync(x => x.Code == "AUTHORIZATION", ct);
        PageGroup? systemGroup = await _pageGroupRepository.FirstOrDefaultAsync(x => x.Code == "SYSTEM", ct);
        PageGroup? reportsGroup = await _pageGroupRepository.FirstOrDefaultAsync(x => x.Code == "REPORTS", ct);

        var pages = new[]
        {
            new { Code = "DASHBOARD_HOME", Name = "Dashboard", Path = "/dashboard", PageGroupId = dashboardGroup?.Id },
            new { Code = "DASHBOARD_ANALYTICS", Name = "Analytics", Path = "/dashboard/analytics", PageGroupId = dashboardGroup?.Id },
            new { Code = "USERS_LIST", Name = "Users List", Path = "/users", PageGroupId = userMgmtGroup?.Id },
            new { Code = "USERS_CREATE", Name = "Create User", Path = "/users/create", PageGroupId = userMgmtGroup?.Id },
            new { Code = "USERS_EDIT", Name = "Edit User", Path = "/users/edit", PageGroupId = userMgmtGroup?.Id },
            new { Code = "USERS_VIEW", Name = "View User", Path = "/users/view", PageGroupId = userMgmtGroup?.Id },
            new { Code = "ROLES_LIST", Name = "Roles List", Path = "/roles", PageGroupId = authGroup?.Id },
            new { Code = "ROLES_CREATE", Name = "Create Role", Path = "/roles/create", PageGroupId = authGroup?.Id },
            new { Code = "ROLES_EDIT", Name = "Edit Role", Path = "/roles/edit", PageGroupId = authGroup?.Id },
            new { Code = "PERMISSIONS_LIST", Name = "Permissions List", Path = "/permissions", PageGroupId = authGroup?.Id },
            new { Code = "PAGES_LIST", Name = "Pages List", Path = "/pages", PageGroupId = authGroup?.Id },
            new { Code = "SYSTEM_SETTINGS", Name = "System Settings", Path = "/system/settings", PageGroupId = systemGroup?.Id },
            new { Code = "SYSTEM_LOGS", Name = "System Logs", Path = "/system/logs", PageGroupId = systemGroup?.Id },
            new { Code = "REPORTS_USER", Name = "User Reports", Path = "/reports/users", PageGroupId = reportsGroup?.Id },
            new { Code = "REPORTS_SYSTEM", Name = "System Reports", Path = "/reports/system", PageGroupId = reportsGroup?.Id }
        };

        foreach (var page in pages)
        {
            Page? existing = await _pageRepository.FirstOrDefaultAsync(x => x.Code == page.Code, ct);
            if (existing == null)
            {
                await _pageRepository.AddAsync(new Page
                {
                    Code = page.Code,
                    Name = page.Name,
                    Path = page.Path,
                    PageGroupId = page.PageGroupId
                }, ct);

                _logger.LogInformation("Seeded page: {Code}", page.Code);
            }
        }
    }
}
