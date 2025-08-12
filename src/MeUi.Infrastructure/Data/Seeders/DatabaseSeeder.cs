using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;
using MeUi.Infrastructure.Data;
using System.Reflection;

namespace MeUi.Infrastructure.Data.Seeders;

public class DatabaseSeeder
{
    private readonly LoginMethodSeeder _loginMethodSeeder;
    private readonly PermissionSeeder _permissionSeeder;
    private readonly TenantPermissionSeeder _tenantPermissionSeeder;
    private readonly SuperRoleSeeder _superRoleSeeder;
    private readonly SuperRolePermissionSeeder _superRolePermissionSeeder;
    private readonly PageGroupSeeder _pageGroupSeeder;
    private readonly PageSeeder _pageSeeder;
    private readonly SuperUserSeeder _superUserSeeder;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(
        LoginMethodSeeder loginMethodSeeder,
        PermissionSeeder permissionSeeder,
        TenantPermissionSeeder tenantPermissionSeeder,
        SuperRoleSeeder superRoleSeeder,
        SuperRolePermissionSeeder superRolePermissionSeeder,
        PageGroupSeeder pageGroupSeeder,
        PageSeeder pageSeeder,
        SuperUserSeeder superUserSeeder,
        IUnitOfWork unitOfWork,
        ILogger<DatabaseSeeder> logger)
    {
        _loginMethodSeeder = loginMethodSeeder;
        _permissionSeeder = permissionSeeder;
        _tenantPermissionSeeder = tenantPermissionSeeder;
        _superRoleSeeder = superRoleSeeder;
        _superRolePermissionSeeder = superRolePermissionSeeder;
        _pageGroupSeeder = pageGroupSeeder;
        _pageSeeder = pageSeeder;
        _superUserSeeder = superUserSeeder;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken ct = default)
    {
        try
        {
            await _loginMethodSeeder.SeedAsync(ct);
            await _unitOfWork.SaveChangesAsync(ct);

            await _permissionSeeder.SeedAsync(ct);
            await _unitOfWork.SaveChangesAsync(ct);

            await _tenantPermissionSeeder.SeedAsync(ct);
            await _unitOfWork.SaveChangesAsync(ct);

            await _superRoleSeeder.SeedAsync(ct);
            await _unitOfWork.SaveChangesAsync(ct);

            await _superRolePermissionSeeder.SeedAsync(ct);
            await _unitOfWork.SaveChangesAsync(ct);

            await _pageGroupSeeder.SeedAsync(ct);
            await _unitOfWork.SaveChangesAsync(ct);

            await _pageSeeder.SeedAsync(ct);
            await _unitOfWork.SaveChangesAsync(ct);

            await _superUserSeeder.SeedAsync(ct);
            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during database seeding");
            throw;
        }
    }
}